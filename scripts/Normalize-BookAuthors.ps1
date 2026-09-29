param(
    [Parameter(Mandatory = $true)][string]$BinDirectory,
    [Parameter(Mandatory = $true)][string]$BackupPath,
    [Parameter(Mandatory = $true)][string]$ImportDataPath,
    [switch]$Commit
)

# Removes credit labels only at the end of each comma-separated contributor.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
function Get-CleanAuthor([string]$Author) {
    $parts = foreach ($part in ($Author -split ',')) {
        ($part.Trim() -replace '\s+(?:외\s+)?(?:지음|엮음|감수|글|그림|원작|편역|옮김|구성)\s*$', '').Trim()
    }
    if (@($parts | Where-Object { [string]::IsNullOrWhiteSpace($_) }).Count -gt 0) {
        throw 'Author normalization would create an empty contributor.'
    }
    return ($parts -join ', ')
}

$connection = $null
$transaction = $null
try {
    $importPath = (Resolve-Path -LiteralPath $ImportDataPath).Path
    $importBooks = Get-Content -LiteralPath $importPath -Raw -Encoding UTF8 | ConvertFrom-Json
    foreach ($book in $importBooks) { $book.Author = Get-CleanAuthor $book.Author }
    $bin = (Resolve-Path -LiteralPath $BinDirectory).Path
    $assembly = [Reflection.Assembly]::LoadFrom((Join-Path $bin 'Book_Management.exe'))
    $config = $assembly.GetType('Book_Management.DatabaseConfig', $true)
    Add-Type -Path (Join-Path $bin 'Microsoft.Data.SqlClient.dll')
    $connection = [Microsoft.Data.SqlClient.SqlConnection]::new($config.GetField('ConnectionString').GetRawConstantValue())
    $connection.Open()
    $transaction = $connection.BeginTransaction()
    $select = $connection.CreateCommand()
    $select.Transaction = $transaction
    $select.CommandText = 'SELECT BookNumber, Author FROM dbo.Books ORDER BY BookNumber;'
    $changes = [Collections.Generic.List[object]]::new()
    $reader = $select.ExecuteReader()
    try {
        while ($reader.Read()) {
            $before = $reader.GetString(1)
            $after = Get-CleanAuthor $before
            if ($before -cne $after) {
                $changes.Add([pscustomobject]@{ BookNumber = $reader.GetInt32(0); Before = $before; After = $after })
            }
        }
    }
    finally { $reader.Dispose(); $select.Dispose() }
    if (-not $Commit) {
        $transaction.Rollback()
        Write-Output "Preview: $($changes.Count) author values would change."
        $changes | ConvertTo-Json -Depth 3
        return
    }
    if (Test-Path -LiteralPath $BackupPath) { throw 'Backup already exists; use a new backup path.' }
    $changes | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath $BackupPath -Encoding UTF8

    $update = $connection.CreateCommand()
    $update.Transaction = $transaction
    $update.CommandText = @'
SET NOCOUNT ON;
UPDATE dbo.Books SET Author = @After
WHERE BookNumber = @Id AND Author COLLATE Latin1_General_100_BIN2 = @Before;
SELECT @@ROWCOUNT;
'@
    [void]$update.Parameters.Add('@Id', [Data.SqlDbType]::Int)
    [void]$update.Parameters.Add('@Before', [Data.SqlDbType]::NVarChar, 100)
    [void]$update.Parameters.Add('@After', [Data.SqlDbType]::NVarChar, 100)
    try {
        foreach ($change in $changes) {
            $update.Parameters['@Id'].Value = $change.BookNumber
            $update.Parameters['@Before'].Value = $change.Before
            $update.Parameters['@After'].Value = $change.After
            if ([int]$update.ExecuteScalar() -ne 1) { throw 'An author changed concurrently; the transaction will be rolled back.' }
        }
    }
    finally { $update.Dispose() }

    $check = $connection.CreateCommand()
    $check.Transaction = $transaction
    $check.CommandText = 'SELECT BookNumber, Author FROM dbo.Books ORDER BY BookNumber;'
    $reader = $check.ExecuteReader()
    $verified = 0
    try {
        while ($reader.Read()) {
            $author = $reader.GetString(1)
            if ($author -cne (Get-CleanAuthor $author)) { throw 'Author cleanup verification failed.' }
            $verified++
        }
    }
    finally { $reader.Dispose(); $check.Dispose() }
    $transaction.Commit()
    Write-Output "Committed $($changes.Count) author updates; verified $verified books."
    # The DB commit result is reported before updating the separate import artifact.
    $importBooks | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $importPath -Encoding UTF8
    Write-Output "Updated $($importBooks.Count) import records."
    $changes | Select-Object -First 5 | ConvertTo-Json -Depth 3
}
catch {
    if ($null -ne $transaction -and $null -ne $transaction.Connection) { $transaction.Rollback() }
    $cause = $_.Exception.GetBaseException()
    if ($cause.GetType().FullName -like '*SqlException') { Write-Error ('Database error; SQL number: ' + $cause.Number) }
    else { Write-Error $_ }
    exit 1
}
finally {
    if ($null -ne $transaction) { $transaction.Dispose() }
    if ($null -ne $connection) { $connection.Dispose() }
}
