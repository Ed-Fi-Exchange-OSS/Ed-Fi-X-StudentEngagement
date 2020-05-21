Import-Module "$PSScriptRoot\DBCmds" -Force
Import-Module "$PSScriptRoot\Prettify" -Force


function Test-HasLogDataStructures($conn){
    # Check if studentinformation table exists
    $database = $conn.Database
    $queryStudentInformation = "Select Count(*) as N From information_schema.tables WHERE table_schema = '$database' AND table_name = 'studentinformation';"    
    $rowCount = Get-QueryScalar $queryStudentInformation $conn

    return ($rowCount -eq 1)
}

function Get-StudentInfoRowCount($conn){
    $database = $conn.Database    
    $query = "Select Count(*) From $database.studentinformation"
    return Get-QueryScalar $query $conn
}

function Write-StudentInfo($conn, $reader){
    $tran = $conn.BeginTransaction()
    try{
        for($i=1; $i -lt $reader.Length; $i++ ){
            $query = $reader[0][0] + $reader[$i][0]
            Execute-NonQuery $query $conn
        }
        $tran.Commit()
    }catch {
        $tran.Rollback()
        throw $_.Exception
    }
}

function Get-StudentInfo($destDatabase, $conn){
    $exportQuery = "DECLARE @tableName as varchar(50) = '$destDatabase.studentinformation'
    Select
        'Insert into ' + @tableName + ' (
            StudentUSI, StudentUniqueId, StudentStateIdentificationCode, IdentityElectronicMailAddress, DeviceId
            , LocalEducationAgencyName, SchoolName, SchoolYear, SchoolCurrentGradeLevelDescriptorCodeValue, SchoolTypeDescriptorCodeValue, ExitWithdrawalDate
            , FirstName, MiddleName, LastSurname, BirthDate, BirthSexDescriptorCodeValue
            , Ethnicity, Race_AmericanIndianAlaskanNative, Race_Asian, Race_BlackAfricaAmerican, Race_NativeHawaiianPacificIslander, Race_White, Race_ChooseNotToRespond, Race_Other
            , DisabilityStatusDescriptorCodeValue, EconomicallyDisadvantageDescriptorCodeValue, ELLStatusDescriptorCodeValue, MigrantDescriptorCodeValue, HomelessDescriptorCodeValue, FosterDescriptorCodeValue, F504DescriptorCodeValue
            , ContactInfoLastSurname, ContactInfoFirstName, ContactInfoRelationToStudent, ContactInfoCellPhoneNumber, ContactInfoElectronicMailAddress
            ) Values '  as cmd
    
    Union ALL
    
    Select '('  
            + Cast (StudentUSI as VarChar(10) ) + ',''' + StudentUniqueId + ''',NULL,''' + ElectronicMailAddress  + ''',NULL' 
            + ',''' + LocalEducationAgencyName + ''',''' + SchoolName + ''', 2020, ''' + EntryGradeLevelDescriptorCodeValue  + ''',''' + 'TODO SchoolTypeDescriptorCodeValue' + ''',' + IsNull('''' + Convert(varchar(100), ExitWithdrawDate, 120 ) + '''', 'NULL') 
            + ',''' + FirstName + ''',' + IsNull('''' + MiddleName + '''','NULL') + ',''' + LastSurname + ''',''' + Convert(varchar(100), BirthDate, 120 ) + ''',''' + SexDescriptorCodeValue + '''' 
            + ',''' + Ethnicity + ''',' + Cast(Race_AmericanIndianAlaskanNative as varchar(2))  + ',' + Cast(Race_Asian as varchar(2)) + ',' + Cast(Race_BlackAfricanAmerican as varchar(2)) + ',' + Cast(Race_NativeHawaiianPacificIslander as varchar(2)) + ',' + Cast(Race_White as varchar(2)) + ',' + Cast(Race_ChooseNotToRespond as varchar(2)) + ',' + Cast(Race_Other as varchar(2))
            + ', NULL, NULL, ' + IsNull('''' + LimitedEnglishProficiency + '''', 'NULL') + ',' + IsNull('''' + MigrantDescriptorCodeValue + '''', 'NULL') + ',' + IsNull('''' + HomelessDescriptorCodeValue + '''', 'NULL') + ', NULL,' + IsNull('''' + F504DescriptorCodeValue + '''', 'NULL') 
            + ',' + IsNull('''' + ContactInfoLastSurname + '''', 'NULL') + ','  + IsNull('''' + ContactInfoFirstName + '''', 'NULL') + ','  + IsNull('''' + ContactInfoRelationToStudent + '''', 'NULL') + ','  + IsNull('''' + ContactInfoCellPhoneNumber + '''', 'NULL') + ','  + IsNull('''' + ContactInfoElectronicMailAddress + '''', 'NULL') 
            + ')' as cmd
        From (
            SELECT
                RowNumber = ROW_NUMBER() over (order by s.StudentUSI)
                -- Basic identity and demogs
                , s.StudentUSI, s.StudentUniqueId, s.FirstName, s.MiddleName, s.LastSurname, BirthDate
                , sd.CodeValue SexDescriptorCodeValue
                , seoaem.ElectronicMailAddress, ed.CodeValue EmailType
                , eo.NameOfInstitution LocalEducationAgencyName, so.NameOfInstitution SchoolName, ssa.EntryDate, gld.CodeValue EntryGradeLevelDescriptorCodeValue, ssa.ExitWithdrawDate
                -- Race and Ethnicity 
                , (CASE WHEN seoa.HispanicLatinoEthnicity=1 then 'Hispanic' ELSE 'Non Hispanic' END) Ethnicity
                , Case when sr1.RaceDescriptorId is not null then 1 else 0 end as 'Race_AmericanIndianAlaskanNative'
                , Case when sr2.RaceDescriptorId is not null then 1 else 0 end as 'Race_Asian'
                , Case when sr3.RaceDescriptorId is not null then 1 else 0 end as 'Race_BlackAfricanAmerican'
                , Case when sr4.RaceDescriptorId is not null then 1 else 0 end as 'Race_ChooseNotToRespond'
                , Case when sr5.RaceDescriptorId is not null then 1 else 0 end as 'Race_NativeHawaiianPacificIslander'
                , Case when sr6.RaceDescriptorId is not null then 1 else 0 end as 'Race_Other'
                , Case when sr7.RaceDescriptorId is not null then 1 else 0 end as 'Race_White'
    
                -- Programs and Indicators (find by SELECT CodeValue from edfi.Descriptor WHERE Namespace like '%ProgramType%')
                , lep.CodeValue LimitedEnglishProficiency
                , (SELECT distinct 'Homeless' from edfi.GeneralStudentProgramAssociation gspa
                    INNER JOIN edfi.Descriptor ptd on gspa.ProgramTypeDescriptorId = ptd.DescriptorId 
                    WHERE ptd.CodeValue like '%Homeless%' and gspa.StudentUSI = s.StudentUSI and gspa.EducationOrganizationId = seoa.EducationOrganizationId) HomelessDescriptorCodeValue
                , (SELECT distinct 'Migrant' from edfi.GeneralStudentProgramAssociation gspa
                    INNER JOIN edfi.Descriptor ptd on gspa.ProgramTypeDescriptorId = ptd.DescriptorId 
                    WHERE ptd.CodeValue like '%Migrant%' and gspa.StudentUSI = s.StudentUSI and gspa.EducationOrganizationId = seoa.EducationOrganizationId) MigrantDescriptorCodeValue
                , (SELECT distinct 'ESL' from edfi.GeneralStudentProgramAssociation gspa
                    INNER JOIN edfi.Descriptor ptd on gspa.ProgramTypeDescriptorId = ptd.DescriptorId 
                    WHERE ptd.CodeValue like '%English as a Second Language%' and gspa.StudentUSI = s.StudentUSI and gspa.EducationOrganizationId = seoa.EducationOrganizationId) ESL
                , (SELECT distinct '504' from edfi.GeneralStudentProgramAssociation gspa
                    INNER JOIN edfi.Descriptor ptd on gspa.ProgramTypeDescriptorId = ptd.DescriptorId 
                    WHERE ptd.CodeValue like '%504%' and gspa.StudentUSI = s.StudentUSI and gspa.EducationOrganizationId = seoa.EducationOrganizationId) F504DescriptorCodeValue
    
                -- Contact info
                , p.LastSurname as ContactInfoLastSurname, p.FirstName as ContactInfoFirstName
                , spad.CodeValue as ContactInfoRelationToStudent
                , pt.TelephoneNumber as ContactInfoCellPhoneNumber
                , pe.ElectronicMailAddress as ContactInfoElectronicMailAddress
                From edfi.Student s
                -- Demogs reported at the district level
                INNER JOIN edfi.StudentEducationOrganizationAssociation seoa on s.StudentUSI = seoa.StudentUSI
                INNER JOIN edfi.EducationOrganization eo on seoa.EducationOrganizationId = eo.EducationOrganizationId
                INNER JOIN edfi.Descriptor sd on seoa.SexDescriptorId = sd.DescriptorId
                LEFT JOIN edfi.Descriptor lep on seoa.LimitedEnglishProficiencyDescriptorId = lep.DescriptorId
                -- Enrollment
                INNER JOIN edfi.StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI
                INNER JOIN edfi.EducationOrganization so on ssa.SchoolId = so.EducationOrganizationId
                INNER JOIN edfi.Descriptor gld on ssa.EntryGradeLevelDescriptorId = gld.DescriptorId
                -- Email (Note: Some students have multiple emails and the rows will duplicate. You can fitlter by a email type.)
                LEFT JOIN (Select *, Rank() Over(Partition by EducationOrganizationId, StudentUsi order by ElectronicMailTypeDescriptorId) as R From edfi.StudentEducationOrganizationAssociationElectronicMail) seoaem on R = 1 and seoa.EducationOrganizationId = seoaem.EducationOrganizationId and seoa.StudentUSI = seoaem.StudentUSI --and seoaem.ElectronicMailTypeDescriptorId = something
                LEFT JOIN edfi.Descriptor ed on seoaem.ElectronicMailTypeDescriptorId = ed.DescriptorId
                -- Race (Note: Ensure you have the right descriptorIds. SELECT * FROM edfi.Descriptor where Namespace like '%Race%')
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr1 ON seoa.EducationOrganizationId = sr1.EducationOrganizationId and s.StudentUSI = sr1.StudentUsi and sr1.RaceDescriptorId = 1868 --'American Indian - Alaskan Native'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr2 ON seoa.EducationOrganizationId = sr2.EducationOrganizationId and s.StudentUSI = sr2.StudentUsi and sr2.RaceDescriptorId = 1869 --'Asian'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr3 ON seoa.EducationOrganizationId = sr3.EducationOrganizationId and s.StudentUSI = sr3.StudentUsi and sr3.RaceDescriptorId = 1870 --'Black - African American'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr4 ON seoa.EducationOrganizationId = sr4.EducationOrganizationId and s.StudentUSI = sr4.StudentUsi and sr4.RaceDescriptorId = 1871 --'Choose Not to Respond'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr5 ON seoa.EducationOrganizationId = sr5.EducationOrganizationId and s.StudentUSI = sr5.StudentUsi and sr5.RaceDescriptorId = 1872 --'Native Hawaiian - Pacific Islander'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr6 ON seoa.EducationOrganizationId = sr6.EducationOrganizationId and s.StudentUSI = sr6.StudentUsi and sr6.RaceDescriptorId = 1873 --'Other'
                LEFT JOIN edfi.StudentEducationOrganizationAssociationRace sr7 ON seoa.EducationOrganizationId = sr7.EducationOrganizationId and s.StudentUSI = sr7.StudentUsi and sr7.RaceDescriptorId = 1874 --'White'
                -- Contact Info
                LEFT JOIN edfi.StudentParentAssociation spa ON s.StudentUSI = spa.StudentUSI and PrimaryContactStatus = 1
                    LEFT JOIN edfi.Parent p ON spa.ParentUSI = p.ParentUSI
                        LEFT JOIN edfi.ParentTelephone pt ON p.ParentUSI = pt.ParentUSI and pt.OrderOfPriority = 1
                        LEFT JOIN (Select *, RANK() over(Partition by ParentUSI order by ElectronicMailTypeDescriptorId) as R from edfi.ParentElectronicMail) pe ON p.ParentUSI = pe.ParentUSI and pe.R = 1
                    LEFT JOIN edfi.Descriptor as spad on spa.RelationDescriptorId = spad.DescriptorId
        ) as Data
    
    "
    $reader = Get-QueryReader $exportQuery $conn
    return $reader
}


function Import-StudentInfo($sourceConnStr, $destConnStr){
    Write-HostStep "Import StudentInfo table data from edfi"

    $dConn = Get-MySQLConnection $destConnStr
    $sConn = get-MSSQLConnection $sourceConnStr

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
        Write-Warning "StudentInformation table has data. Cleaning"
        $truncateQuery = "TRUNCATE TABLE " + $dConn.Database + ".StudentInformation"
        $null = Execute-NonQuery $truncateQuery $dConn
    }
    Write-Host "* Destination StudentInformation is empty"

    Write-Host "* Exporting data from source"
    $rStInfo = Get-StudentInfo $dConn.Database $sConn
    
    Write-Host "* Importinging data into destination, please wait"
    $null = Write-StudentInfo $dConn $rStInfo

    Write-HostStep "Import StudentInfo data completed"

    $sConn.Dispose()
    $dConn.Dispose()
    
}