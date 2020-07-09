DECLARE @tableName as varchar(50) = 'public."StudentInformation"'
    Select
        'Insert into ' + @tableName + ' (
            "StudentUSI", "StudentUniqueId", "StudentStateIdentificationCode", "IdentityElectronicMailAddress", "DeviceId"
            , "LocalEducationAgencyName", "SchoolName", "SchoolYear", "SchoolCurrentGradeLevelDescriptorCodeValue", "SchoolTypeDescriptorCodeValue", "ExitWithdrawalDate"
            , "FirstName", "MiddleName", "LastSurname", "BirthDate", "BirthSexDescriptorCodeValue"
            , "Ethnicity", "Race_AmericanIndianAlaskanNative", "Race_Asian", "Race_BlackAfricaAmerican", "Race_NativeHawaiianPacificIslander", "Race_White", "Race_ChooseNotToRespond", "Race_Other"
            , "DisabilityStatusDescriptorCodeValue", "EconomicallyDisadvantageDescriptorCodeValue", "ELLStatusDescriptorCodeValue", "MigrantDescriptorCodeValue", "HomelessDescriptorCodeValue", "FosterDescriptorCodeValue", "F504DescriptorCodeValue"
            , "ContactInfoLastSurname", "ContactInfoFirstName", "ContactInfoRelationToStudent", "ContactInfoCellPhoneNumber", "ContactInfoElectronicMailAddress"
            ) Values '  as cmd
    
    Union ALL
    
    Select '('  
            + Cast (StudentUSI as VarChar(10) ) + ',''' + StudentUniqueId + ''',NULL,''' + ElectronicMailAddress  + ''',NULL' 
            + ',''' + LocalEducationAgencyName + ''',''' + SchoolName + ''', 2020, ''' + EntryGradeLevelDescriptorCodeValue  + ''',''' + 'TODO SchoolTypeDescriptorCodeValue' + ''',' + IsNull('''' + Convert(varchar(100), ExitWithdrawDate, 120 ) + '''', 'NULL') 
            + ',''' + FirstName + ''',' + IsNull('''' + MiddleName + '''','NULL') + ',''' + LastSurname + ''',''' + Convert(varchar(100), BirthDate, 120 ) + ''',''' + SexDescriptorCodeValue + '''' 
            + ',''' + Ethnicity + ''',' + Cast(Race_AmericanIndianAlaskanNative as varchar(5))  + ',' + Cast(Race_Asian as varchar(5)) + ',' + Cast(Race_BlackAfricanAmerican as varchar(5)) + ',' + Cast(Race_NativeHawaiianPacificIslander as varchar(5)) + ',' + Cast(Race_White as varchar(5)) + ',' + Cast(Race_ChooseNotToRespond as varchar(5)) + ',' + Cast(Race_Other as varchar(5))
            + ', NULL, NULL, ' + IsNull('''' + LimitedEnglishProficiency + '''', 'NULL') + ',' + IsNull('''' + MigrantDescriptorCodeValue + '''', 'NULL') + ',' + IsNull('''' + HomelessDescriptorCodeValue + '''', 'NULL') + ', NULL,' + IsNull('''' + F504DescriptorCodeValue + '''', 'NULL') 
            + ',' + IsNull('''' + ContactInfoLastSurname + '''', 'NULL') + ','  + IsNull('''' + ContactInfoFirstName + '''', 'NULL') + ','  + IsNull('''' + ContactInfoRelationToStudent + '''', 'NULL') + ','  + IsNull('''' + ContactInfoCellPhoneNumber + '''', 'NULL') + ','  + IsNull('''' + ContactInfoElectronicMailAddress + '''', 'NULL') 
            + ')' as cmd
        From (
            SELECT
                RowNumber = ROW_NUMBER() over (order by s.StudentUSI)
                -- Basic identity and demogs
                , s.StudentUSI, s.StudentUniqueId, s.FirstName, s.MiddleName, s.LastSurname, BirthDate
                , sxt.CodeValue SexDescriptorCodeValue
                , seoaem.ElectronicMailAddress, emt.CodeValue EmailType
                , eo.NameOfInstitution LocalEducationAgencyName, so.NameOfInstitution SchoolName, ssa.EntryDate, gld.CodeValue EntryGradeLevelDescriptorCodeValue, ssa.ExitWithdrawDate
                -- Race and Ethnicity 
                , (CASE WHEN s.HispanicLatinoEthnicity = 1 then 'Hispanic' ELSE 'Non Hispanic' END) Ethnicity
                , Race_AmericanIndianAlaskanNative
                , Race_Asian
                , Race_BlackAfricanAmerican
                , Race_ChooseNotToRespond
                , Race_NativeHawaiianPacificIslander
                , Race_Other
                , Race_White
    
                -- Programs and Indicators (find by SELECT CodeValue from edfi.Descriptor WHERE Namespace like '%ProgramType%')
                , lep.CodeValue LimitedEnglishProficiency
                , (SELECT distinct 'Homeless' from [edfi].[StudentCharacteristic] gspa
					inner join [edfi].[StudentCharacteristicDescriptor] schd on gspa.StudentCharacteristicDescriptorId = schd.StudentCharacteristicDescriptorId
					inner join [edfi].[StudentCharacteristicType] scht on schd.StudentCharacteristicTypeId = scht.StudentCharacteristicTypeId
                    WHERE scht.CodeValue like '%Homeless%') HomelessDescriptorCodeValue
                , (SELECT distinct 'Migrant' from [edfi].[StudentCharacteristic] gspa
					inner join [edfi].[StudentCharacteristicDescriptor] schd on gspa.StudentCharacteristicDescriptorId = schd.StudentCharacteristicDescriptorId
					inner join [edfi].[StudentCharacteristicType] scht on schd.StudentCharacteristicTypeId = scht.StudentCharacteristicTypeId
                    WHERE scht.CodeValue like '%Migrant%') MigrantDescriptorCodeValue
                , (SELECT distinct '504' from [edfi].[StudentCharacteristic] gspa
					inner join [edfi].[StudentCharacteristicDescriptor] schd on gspa.StudentCharacteristicDescriptorId = schd.StudentCharacteristicDescriptorId
					inner join [edfi].[StudentCharacteristicType] scht on schd.StudentCharacteristicTypeId = scht.StudentCharacteristicTypeId
                    WHERE scht.CodeValue like '%504%') F504DescriptorCodeValue                
    
                -- Contact info
                , p.LastSurname as ContactInfoLastSurname, p.FirstName as ContactInfoFirstName
                , relt.CodeValue as ContactInfoRelationToStudent
                , pt.TelephoneNumber as ContactInfoCellPhoneNumber
                , pe.ElectronicMailAddress as ContactInfoElectronicMailAddress
                From edfi.Student s
                -- Demogs reported at the district level
                INNER JOIN edfi.StudentSchoolAssociation seoa on s.StudentUSI = seoa.StudentUSI				
                INNER JOIN edfi.EducationOrganization eo on seoa.EducationOrganizationId = eo.EducationOrganizationId
				LEFT JOIN edfi.SexType sxt on s.SexTypeId = sxt.SexTypeId
                LEFT JOIN edfi.Descriptor lep on s.LimitedEnglishProficiencyDescriptorId = lep.DescriptorId
                -- Enrollment
                INNER JOIN edfi.StudentSchoolAssociation ssa on s.StudentUSI = ssa.StudentUSI
                INNER JOIN edfi.EducationOrganization so on ssa.SchoolId = so.EducationOrganizationId
                INNER JOIN edfi.Descriptor gld on ssa.EntryGradeLevelDescriptorId = gld.DescriptorId
                -- Email (Note: Some students have multiple emails and the rows will duplicate. You can fitlter by a email type.)
                LEFT JOIN (Select *, Rank() Over(Partition by StudentUsi order by ElectronicMailTypeId) as R From edfi.StudentElectronicMail) seoaem on R = 1 and seoa.StudentUSI = seoaem.StudentUSI 
				LEFT JOIN edfi.ElectronicMailType emt on seoaem.ElectronicMailTypeId = emt.ElectronicMailTypeId
                
				-- Race (Note: Ensure you have the right descriptorIds. SELECT * FROM edfi.Descriptor where Namespace like '%Race%')
				LEFT JOIN (Select StudentUSI
					, Case When [1] = 1 Then 'TRUE' else 'FALSE' End AS Race_AmericanIndianAlaskanNative
					, Case When [2] = 1 Then 'TRUE' else 'FALSE' End AS Race_Asian
					, Case When [3] = 1 Then 'TRUE' else 'FALSE' End AS Race_BlackAfricanAmerican
					, Case When [4] = 1 Then 'TRUE' else 'FALSE' End AS Race_ChooseNotToRespond
					, Case When [5] = 1 Then 'TRUE' else 'FALSE' End AS Race_NativeHawaiianPacificIslander
					, Case When [6] = 1 Then 'TRUE' else 'FALSE' End AS Race_Other
					, Case When [7] = 1 Then 'TRUE' else 'FALSE' End AS Race_White
					from (select StudentUSI, sr.RaceTypeId, CodeValue
						from [edfi].[StudentRace] as sr 
						inner join [edfi].RaceType rt on sr.RaceTypeId = rt.RaceTypeId) as source
					PIVOT  
					(  
					Count(CodeValue)  
					FOR RaceTypeId IN ([1], [2], [3], [4], [5], [6], [7])  
					) AS PivotTable
				) as sr on s.StudentUSI = sr.StudentUSI
                -- Contact Info
                LEFT JOIN edfi.StudentParentAssociation spa ON s.StudentUSI = spa.StudentUSI and PrimaryContactStatus = 1
                    LEFT JOIN edfi.Parent p ON spa.ParentUSI = p.ParentUSI
                        LEFT JOIN edfi.ParentTelephone pt ON p.ParentUSI = pt.ParentUSI and pt.OrderOfPriority = 1
                        LEFT JOIN (Select *, RANK() over(Partition by ParentUSI order by ElectronicMailTypeId) as R from edfi.ParentElectronicMail) pe ON p.ParentUSI = pe.ParentUSI and pe.R = 1
                    LEFT JOIN edfi.RelationType relt on spa.RelationTypeId = relt.RelationTypeId
        ) as Data