Import-Module "$PSScriptRoot\DBCmds" -Force -DisableNameChecking
Import-Module "$PSScriptRoot\Prettify" -Force


function Test-HasLogDataStructures($conn) {
    # Check if studentinformation table exists
    $queryStudentInformation = "Select Count(*) as N From information_schema.tables WHERE upper(table_name) = upper('StudentInformation');"
    $rowCount = Get-QueryScalar $queryStudentInformation $conn

    return ($rowCount -eq 1)
}

function Get-StudentInfoRowCount($conn) {
    $query = "Select Count(*) From ""StudentInformation"""
    return Get-QueryScalar $query $conn
}

function Write-StudentInfo($conn, $reader) {
    $tran = $conn.BeginTransaction()
    try {
        for ($i = 1; $i -lt $reader.Length; $i++ ) {
            $query = $reader[0][0] + $reader[$i][0]
            Execute-NonQuery $query $conn $tran
            Write-Host -NoNewline "."
        }
        $tran.Commit()
        Write-Host
    }
    catch {
        $tran.Rollback()
        throw $_.Exception
    }
}

function Import-StudentInfo($sourceConnStr, $destConnStr, $exportQuery) {
    Write-Host "Import StudentInformation table data from edfi"

    try {

        $dConn = Get-PostGreSQLConnection $destConnStr
        Write-Host "Connected to Destination"
        $sConn = get-MSSQLConnection $sourceConnStr
        Write-Host "Connected to Source"

        $testDS = Test-HasLogDataStructures $dConn
        if (-not $testDS ) {    
            Write-Warning "Table StudentInformation don't exists. Aborting"
            $sConn.Dispose()
            $dConn.Dispose()    
            return
        }
        Write-Host "* Destination have StudentInformation table"

        $testRowCount = Get-StudentInfoRowCount $dConn
        if ($testRowCount -gt 0) {    
            Write-Warning "StudentInformation table have data. Cleaning"
            $truncateQuery = "TRUNCATE TABLE ""StudentInformation"""
            $null = Execute-NonQuery $truncateQuery $dConn
        }
        Write-Host "* Destination StudentInformation is empty"

        Write-Host "* Exporting data from source"
        $rStInfo = Get-QueryReader $exportQuery $sConn

        Write-Host "* Importinging data into destination, please wait"
        $null = Write-StudentInfo $dConn $rStInfo

        Write-HostStep "Import StudentInfo data completed"
    }
    catch {            
        Write-Warning "An exception happen while importing data. Terminating"
        Write-Warning $_.Exception.GetType().FullName
        Write-Warning $_.Exception.Message
    }
    finally {
        if ($null -ne $sConn) { $sConn.Dispose() }
        if ($null -ne $dConn) { $dConn.Dispose() }
    }
}