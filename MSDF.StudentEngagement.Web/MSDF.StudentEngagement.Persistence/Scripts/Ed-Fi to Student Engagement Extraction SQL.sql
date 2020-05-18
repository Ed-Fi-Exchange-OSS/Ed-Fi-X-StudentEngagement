DECLARE @tableName as varchar(50) = '1studentlearningevents.studentinformation'
Select
	'Insert into ' + @tableName + ' (
		StudentUSI, StudentUniqueId, StudentStateIdentificationCode, IdentityElectronicMailAddress, DeviceId
		, LocalEducationAgencyName, SchoolName, SchoolYear, SchoolCurrentGradeLevelDescriptorCodeValue, SchoolTypeDescriptorCodeValue, ExitWithdrawalDate
		, FirstName, MiddleName, LastSurname, BirthDate, BirthSexDescriptorCodeValue
		, Ethnicity, Race_AmericanIndianAlaskanNative, Race_Asian, Race_BlackAfricaAmerican, Race_NativeHawaiianPacificIslander, Race_White, Race_ChooseNotToRespond, Race_Other
		, DisabilityStatusDescriptorCodeValue, EconomicallyDisadvantageDescriptorCodeValue, ELLStatusDescriptorCodeValue, MigrantDescriptorCodeValue, HomelessDescriptorCodeValue, FosterDescriptorCodeValue, F504DescriptorCodeValue
		) Values ( ' 
		+ Cast (StudentUSI as VarChar(10) ) + ',''' + StudentUniqueId + ''',NULL,''' + ElectronicMailAddress  + ''',NULL' 
		+ ',''' + LocalEducationAgencyName + ''',''' + SchoolName + ''', 2020, ''' + EntryGradeLevelDescriptorCodeValue  + ''',''' + 'TODO SchoolTypeDescriptorCodeValue' + ''',' + IsNull('''' + Convert(varchar(100), ExitWithdrawDate, 120 ) + '''', 'NULL') 
		+ ',''' + FirstName + ''',' + IsNull('''' + MiddleName + '''','NULL') + ',''' + LastSurname + ''',''' + Convert(varchar(100), BirthDate, 120 ) + ''',''' + SexDescriptorCodeValue + '''' 
		+ ',''' + Ethnicity + ''',' + Cast(Race_AmericanIndianAlaskanNative as varchar(2))  + ',' + Cast(Race_Asian as varchar(2)) + ',' + Cast(Race_BlackAfricanAmerican as varchar(2)) + ',' + Cast(Race_NativeHawaiianPacificIslander as varchar(2)) + ',' + Cast(Race_White as varchar(2)) + ',' + Cast(Race_ChooseNotToRespond as varchar(2)) + ',' + Cast(Race_Other as varchar(2))
		+ ', NULL, NULL, ' + IsNull('''' + LimitedEnglishProficiency + '''', 'NULL') + ',' + IsNull('''' + MigrantDescriptorCodeValue + '''', 'NULL') + ',' + IsNull('''' + HomelessDescriptorCodeValue + '''', 'NULL') + ', NULL,' + IsNull('''' + F504DescriptorCodeValue + '''', 'NULL') 
		+ ');
	' as cmd
	/*,
	StudentUSI, StudentUniqueId, FirstName, MiddleName, LastSurname, BirthDate, SexDescriptorCodeValue
	, ElectronicMailAddress, EmailType
	, LocalEducationAgencyName, SchoolName, EntryDate, EntryGradeLevelDescriptorCodeValue, ExitWithdrawDate
	, Ethnicity, Race_AmericanIndianAlaskanNative, Race_Asian, Race_BlackAfricanAmerican, Race_NativeHawaiianPacificIslander, Race_White, Race_ChooseNotToRespond, Race_Other
	, LimitedEnglishProficiency, HomelessDescriptorCodeValue, MigrantDescriptorCodeValue, ESL, F504DescriptorCodeValue
	*/
From (
SELECT 
	-- Basic identity and demogs
	s.StudentUSI, s.StudentUniqueId, s.FirstName, s.MiddleName, s.LastSurname, BirthDate
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
	
	) as Data

