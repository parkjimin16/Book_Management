param(
    [string]$BinDirectory = (Join-Path $PSScriptRoot '..\Book_Management\bin\Debug')
)

# Run after the English-name migration and a successful build, using Windows PowerShell 5.1.
# Only existing repository read methods are called. No rows or credentials are printed.
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$script:queryCount = 0
$script:stage = 'Initialize'
$originalPath = $env:PATH

function Assert-Condition([bool]$Condition, [string]$Description) {
    if (-not $Condition) {
        $script:stage = $Description
        throw [InvalidOperationException]::new('Verification assertion failed.')
    }
}

function Assert-Columns([System.Data.DataTable]$Table, [string[]]$Expected) {
    Assert-Condition ($null -ne $Table) 'Expected DataTable'
    Assert-Condition ($Table.Columns.Count -eq $Expected.Count) 'Column count'
    for ($index = 0; $index -lt $Expected.Count; $index++) {
        Assert-Condition ($Table.Columns[$index].ColumnName -ceq $Expected[$index]) 'Korean UI column aliases'
    }
}

function Invoke-Read([object]$Repository, [string]$Method, [object[]]$Arguments) {
    $script:stage = $Repository.GetType().Name + '.' + $Method
    $result = [EnglishSchemaReadHarness]::Invoke($Repository, $Method, $Arguments)
    $script:queryCount++
    return ,$result
}

try {
    Assert-Condition ($PSVersionTable.PSEdition -eq 'Desktop') 'Windows PowerShell 5.1 is required'
    $bin = (Resolve-Path -LiteralPath $BinDirectory).Path
    $exe = Join-Path $bin 'Book_Management.exe'
    Assert-Condition (Test-Path -LiteralPath $exe -PathType Leaf) 'Built executable exists'
    $env:PATH = $bin + ';' + $originalPath

    # A managed resolver also works when dependencies load on an async worker thread.
    Add-Type -TypeDefinition @'
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

public static class EnglishSchemaReadHarness
{
    private static string directory;
    private static ResolveEventHandler resolver;

    public static Assembly Load(string path)
    {
        directory = Path.GetDirectoryName(path);
        resolver = delegate(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            string dependency = Path.Combine(directory, name + ".dll");
            return File.Exists(dependency) ? Assembly.LoadFrom(dependency) : null;
        };
        AppDomain.CurrentDomain.AssemblyResolve += resolver;
        return Assembly.LoadFrom(path);
    }

    public static object Invoke(object repository, string method, object[] arguments)
    {
        MethodInfo info = repository.GetType().GetMethod(method);
        if (info == null) throw new MissingMethodException(method);
        Task task = (Task)info.Invoke(repository, arguments);
        // This is a console verification process, with no WinForms UI context.
        task.GetAwaiter().GetResult();
        PropertyInfo result = task.GetType().GetProperty("Result");
        return result == null ? null : result.GetValue(task, null);
    }

    public static void Cleanup()
    {
        if (resolver != null) AppDomain.CurrentDomain.AssemblyResolve -= resolver;
    }
}
'@

    $assembly = [EnglishSchemaReadHarness]::Load($exe)
    $adminType = $assembly.GetType('Book_Management.Book.AdminBookRepository', $true)
    $userType = $assembly.GetType('Book_Management.UserBookRepository', $true)
    $memberType = $assembly.GetType('Book_Management.MemberRepository', $true)
    $pageType = $assembly.GetType('Book_Management.Book.AdminPage', $true)
    $admin = [Activator]::CreateInstance($adminType, $true)
    $user = [Activator]::CreateInstance($userType, $true)
    $member = [Activator]::CreateInstance($memberType, $true)

    $basicColumns = @('제목', '저자', '출판사', '발행연도', '카테고리', 'ISBN')
    $adminColumns = @{
        Books = @('관리번호') + $basicColumns + @('대출가능여부', '대출자')
        Members = @('회원번호', '이름', '연락처', '아이디', '회원코드', '대출현황')
        Requests = $basicColumns
    }
    $userColumns = @('관리번호') + $basicColumns + @('대출가능여부', '대출여부', '반납일', '조회수')
    $nonexistentId = 'verify_' + [Guid]::NewGuid().ToString('N')
    $nonexistentPhone = 'v' + [Guid]::NewGuid().ToString('N').Substring(0, 19)
    $existingBookNumber = $null
    $adminStart = $script:queryCount

    foreach ($pageName in @('Books', 'Members', 'Requests')) {
        $page = [Enum]::Parse($pageType, $pageName)
        $searchColumns = $adminType.GetMethod('GetSearchColumns').Invoke($null, @($page))
        $table = Invoke-Read $admin 'GetList' @($page, $searchColumns[0], '')
        Assert-Columns $table $adminColumns[$pageName]
        if ($pageName -eq 'Books' -and $table.Rows.Count -gt 0) {
            $existingBookNumber = [int]$table.Rows[0]['관리번호']
        }
        $table.Dispose()

        foreach ($searchColumn in $searchColumns) {
            $table = Invoke-Read $admin 'GetList' @($page, $searchColumn, $nonexistentId)
            Assert-Columns $table $adminColumns[$pageName]
            $table.Dispose()
        }
    }
    Write-Host ('PASS admin lists and all search columns: {0} queries' -f ($script:queryCount - $adminStart))

    $userStart = $script:queryCount
    foreach ($mode in 0..3) {
        foreach ($available in @($true, $false)) {
            $keyword = if ($mode -eq 0) { '' } else { $nonexistentId }
            $table = Invoke-Read $user 'Search' @([int]$mode, $keyword, $null, '전체', $available)
            Assert-Columns $table $userColumns
            foreach ($row in $table.Rows) {
                Assert-Condition ([bool]$row['대출가능여부'] -eq $available) 'Availability filter'
            }
            $table.Dispose()
        }
    }

    $table = Invoke-Read $user 'Search' @(0, '', [int16]2025, '문학', $true)
    Assert-Columns $table $userColumns
    foreach ($row in $table.Rows) {
        Assert-Condition (([int]$row['발행연도'] -eq 2025) -and ($row['카테고리'] -eq '문학')) 'Year and category filters'
    }
    $table.Dispose()

    if ($null -ne $existingBookNumber) {
        $detail = Invoke-Read $user 'GetDetail' @([int]$existingBookNumber, $false)
        Assert-Condition ($null -ne $detail) 'Existing book detail'
        Assert-Columns $detail.Table $userColumns
        Assert-Condition ([int]$detail['관리번호'] -eq $existingBookNumber) 'Detail primary key'
        $detail.Table.Dispose()
    }
    else {
        Write-Host 'SKIP existing book detail: no books are present'
    }

    Assert-Condition (-not (Invoke-Read $member 'IsIdExists' @($nonexistentId))) 'Random login ID must not exist'
    $table = Invoke-Read $user 'GetMyLoans' @($nonexistentId)
    Assert-Columns $table $userColumns
    Assert-Condition ($table.Rows.Count -eq 0) 'Nonexistent member has no loans'
    $table.Dispose()
    Write-Host ('PASS user search, detail and loans: {0} queries' -f ($script:queryCount - $userStart - 1))

    Assert-Condition (-not (Invoke-Read $member 'IsPhoneExists' @($nonexistentPhone))) 'Random phone must not exist'
    $login = Invoke-Read $member 'Login' @($nonexistentId, [Guid]::NewGuid().ToString('N'))
    Assert-Condition ($null -eq $login) 'Nonexistent member login must fail'
    Write-Host 'PASS member duplicate checks and nonexistent login: 3 queries'
    Write-Host ('PASS total: {0} repository read queries; no write methods invoked' -f $script:queryCount)
}
catch {
    $errorRecord = $_
    $failure = $_.Exception.GetBaseException()
    $code = if ($null -ne $failure.PSObject.Properties['Number']) { '; SQL number ' + $failure.Number } else { '' }
    # Do not print the raw exception message: it can include connection or row data.
    Write-Host ('FAIL at {0}: {1}{2}' -f $script:stage, $failure.GetType().FullName, $code)
    Write-Host ('Diagnostic: line {0}; error ID {1}; completed queries {2}' -f
        $errorRecord.InvocationInfo.ScriptLineNumber,
        ($errorRecord.FullyQualifiedErrorId -replace '[^A-Za-z0-9._,\-]', '?'),
        $script:queryCount)
    exit 1
}
finally {
    if ('EnglishSchemaReadHarness' -as [type]) {
        [EnglishSchemaReadHarness]::Cleanup()
    }
    $env:PATH = $originalPath
}
