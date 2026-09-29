param(
    [Parameter(Mandatory = $true)][string]$DataPath,
    [switch]$Commit,
    [string]$BinDirectory
)

# Windows PowerShell 5.1. Default mode validates and reads the DB without inserting.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$connection = $null
$transaction = $null
try {
    if ([string]::IsNullOrWhiteSpace($BinDirectory)) {
        $BinDirectory = Join-Path $PSScriptRoot '..\Book_Management\bin\Debug'
    }
    $books = Get-Content -LiteralPath $DataPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $books = @($books)
    if ($books.Count -eq 0) { throw 'The import file contains no books.' }
    $categories = @('총류','철학','종교','사회과학','자연과학','기술과학','예술','언어','문학','역사')
    $seen = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($book in $books) {
        foreach ($field in @('Title','Author','Publisher','Category','Isbn')) {
            if ([string]::IsNullOrWhiteSpace([string]$book.$field)) { throw "Row $($book.Row): missing $field." }
        }
        if ($book.Title.Length -gt 200 -or $book.Author.Length -gt 100 -or $book.Publisher.Length -gt 100 -or $categories -cnotcontains $book.Category) {
            throw "Row $($book.Row): invalid text length or category."
        }
        $year = 0
        if (-not [int]::TryParse([string]$book.PublicationYear, [ref]$year) -or $year -lt 1 -or $year -gt 9999) { throw "Row $($book.Row): invalid year." }
        if ($book.Isbn -cnotmatch '\A(?:[0-9]{13}|[0-9]{9}[0-9X])\z' -or -not $seen.Add($book.Isbn)) { throw "Row $($book.Row): invalid or repeated ISBN." }
    }

    $bin = (Resolve-Path -LiteralPath $BinDirectory).Path
    $assembly = [Reflection.Assembly]::LoadFrom((Join-Path $bin 'Book_Management.exe'))
    $config = $assembly.GetType('Book_Management.DatabaseConfig', $true)
    $connectionString = $config.GetField('ConnectionString').GetRawConstantValue()
    Add-Type -Path (Join-Path $bin 'Microsoft.Data.SqlClient.dll')
    $connection = [Microsoft.Data.SqlClient.SqlConnection]::new($connectionString)
    $connection.Open()

    if ($Commit) { $transaction = $connection.BeginTransaction() }
    $query = $connection.CreateCommand()
    $query.Transaction = $transaction
    # Keep the duplicate check and all inserts together, including concurrent UI inserts.
    $query.CommandText = if ($Commit) { 'SELECT Isbn FROM dbo.Books WITH (TABLOCKX, HOLDLOCK);' } else { 'SELECT Isbn FROM dbo.Books;' }
    $reader = $query.ExecuteReader()
    $existing = [Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    try { while ($reader.Read()) { [void]$existing.Add($reader.GetString(0).Trim()) } }
    finally { $reader.Dispose(); $query.Dispose() }
    $duplicates = @($books | Where-Object { $existing.Contains($_.Isbn) })
    if ($duplicates.Count -gt 0) { throw "Existing ISBNs found in $($duplicates.Count) rows. Nothing was inserted." }

    if (-not $Commit) {
        Write-Output "Validated $($books.Count) books; existing ISBN matches: 0; database changes: 0."
        return
    }

    $insert = $connection.CreateCommand()
    $insert.Transaction = $transaction
    $insert.CommandText = @'
INSERT INTO dbo.Books
    (Title, Author, Publisher, PublicationYear, Category, Isbn,
     IsAvailable, BorrowerLoginId, ViewCount, DueDate)
OUTPUT INSERTED.BookNumber
VALUES (@Title, @Author, @Publisher, @Year, @Category, @Isbn, 1, NULL, 0, NULL);
'@
    [void]$insert.Parameters.Add('@Title', [Data.SqlDbType]::NVarChar, 200)
    [void]$insert.Parameters.Add('@Author', [Data.SqlDbType]::NVarChar, 100)
    [void]$insert.Parameters.Add('@Publisher', [Data.SqlDbType]::NVarChar, 100)
    [void]$insert.Parameters.Add('@Year', [Data.SqlDbType]::SmallInt)
    [void]$insert.Parameters.Add('@Category', [Data.SqlDbType]::NVarChar, 50)
    [void]$insert.Parameters.Add('@Isbn', [Data.SqlDbType]::VarChar, 13)
    $inserted = [Collections.Generic.List[object]]::new()
    try {
        foreach ($book in $books) {
            foreach ($field in @('Title','Author','Publisher','Category','Isbn')) { $insert.Parameters['@' + $field].Value = $book.$field }
            $insert.Parameters['@Year'].Value = [int16]$book.PublicationYear
            $id = [int]$insert.ExecuteScalar()
            $inserted.Add([pscustomobject]@{ BookNumber = $id; Isbn = $book.Isbn; Title = $book.Title })
        }
    }
    finally { $insert.Dispose() }

    $verify = $connection.CreateCommand()
    $verify.Transaction = $transaction
    $verify.CommandText = @'
SELECT COUNT(*) FROM dbo.Books
WHERE BookNumber = @Id AND Isbn = @Isbn AND Title = @Title AND Author = @Author
AND Publisher = @Publisher AND PublicationYear = @Year AND Category = @Category
AND IsAvailable = 1 AND BorrowerLoginId IS NULL AND ViewCount = 0 AND DueDate IS NULL;
'@
    [void]$verify.Parameters.Add('@Id', [Data.SqlDbType]::Int)
    [void]$verify.Parameters.Add('@Isbn', [Data.SqlDbType]::VarChar, 13)
    [void]$verify.Parameters.Add('@Title', [Data.SqlDbType]::NVarChar, 200)
    [void]$verify.Parameters.Add('@Author', [Data.SqlDbType]::NVarChar, 100)
    [void]$verify.Parameters.Add('@Publisher', [Data.SqlDbType]::NVarChar, 100)
    [void]$verify.Parameters.Add('@Year', [Data.SqlDbType]::SmallInt)
    [void]$verify.Parameters.Add('@Category', [Data.SqlDbType]::NVarChar, 50)
    try {
        for ($index = 0; $index -lt $books.Count; $index++) {
            $book = $books[$index]
            $verify.Parameters['@Id'].Value = $inserted[$index].BookNumber
            foreach ($field in @('Isbn','Title','Author','Publisher','Category')) { $verify.Parameters['@' + $field].Value = $book.$field }
            $verify.Parameters['@Year'].Value = [int16]$book.PublicationYear
            if ([int]$verify.ExecuteScalar() -ne 1) { throw "Verification failed for row $($book.Row)." }
        }
    }
    finally { $verify.Dispose() }
    $transaction.Commit()
    Write-Output "Committed and verified $($inserted.Count) books."
    $inserted | ConvertTo-Json -Depth 3
}
catch {
    if ($null -ne $transaction -and $null -ne $transaction.Connection) { $transaction.Rollback() }
    # SQL exception text may contain sensitive connection details or row data.
    if ($_.Exception.GetBaseException().GetType().FullName -like '*SqlException') {
        Write-Error ('Database import failed; SQL number: ' + $_.Exception.GetBaseException().Number)
    }
    else { Write-Error $_ }
    exit 1
}
finally {
    if ($null -ne $transaction) { $transaction.Dispose() }
    if ($null -ne $connection) { $connection.Dispose() }
}
