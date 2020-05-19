CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `db_9aa830_learnev`.`studentengagementreport_todaysengagementbyurl` AS
SELECT
    `db_9aa830_learnev`.`studentlearningeventlog`.`StudentElectronicMailAddress` AS `StudentElectronicMailAddress`
    , `db_9aa830_learnev`.`studentlearningeventlog`.`LeaningAppUrl` AS `LeaningAppUrl`
    , `db_9aa830_learnev`.`studentlearningeventlog`.`UTCStartDate` AS `UTCStartDate`
    , MAX(`db_9aa830_learnev`.`studentlearningeventlog`.`UTCEndDate`) AS `UTCEndDate`
    , MAX(`db_9aa830_learnev`.`studentlearningeventlog`.`TimeSpent`) AS `TimeSpent`
FROM
    `db_9aa830_learnev`.`studentlearningeventlog`
WHERE
    (`db_9aa830_learnev`.`studentlearningeventlog`.`UTCStartDate` > CURDATE())
GROUP BY
    `db_9aa830_learnev`.`studentlearningeventlog`.`StudentElectronicMailAddress`
    , `db_9aa830_learnev`.`studentlearningeventlog`.`LeaningAppUrl`
    , `db_9aa830_learnev`.`studentlearningeventlog`.`UTCStartDate`
    ;



CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `db_9aa830_learnev`.`studentengagementreport_todaysengagement` AS
SELECT
    `sturl`.`StudentElectronicMailAddress` AS `StudentElectronicMailAddress`
    , COUNT(0) AS `URLsVisitedToday`
    , AVG(`sturl`.`TimeSpent`) AS `AVGTimeSpent`
FROM
    `db_9aa830_learnev`.`studentengagementreport_todaysengagementbyurl` `sturl`
GROUP BY
    `sturl`.`StudentElectronicMailAddress`
    ;



CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `db_9aa830_learnev`.`studentengagementreport` AS
SELECT
    `st`.`Id` AS `Id`
    , `st`.`StudentUSI` AS `StudentUSI`
    , `st`.`StudentUniqueId` AS `StudentUniqueId`
    , `st`.`StudentStateIdentificationCode` AS `StudentStateIdentificationCode`
    , `st`.`IdentityElectronicMailAddress` AS `IdentityElectronicMailAddress`
    , `st`.`DeviceId` AS `DeviceId`
    , `st`.`LocalEducationAgencyName` AS `LocalEducationAgencyName`
    , `st`.`SchoolName` AS `SchoolName`
    , `st`.`SchoolYear` AS `SchoolYear`
    , `st`.`SchoolCurrentGradeLevelDescriptorCodeValue` AS `SchoolCurrentGradeLevelDescriptorCodeValue`
    , `st`.`SchoolTypeDescriptorCodeValue` AS `SchoolTypeDescriptorCodeValue`
    , `st`.`ExitWithdrawalDate` AS `ExitWithdrawalDate`
    , `st`.`FirstName` AS `FirstName`
    , `st`.`MiddleName` AS `MiddleName`
    , `st`.`LastSurname` AS `LastSurname`
    , `st`.`BirthDate` AS `BirthDate`
    , `st`.`BirthSexDescriptorCodeValue` AS `BirthSexDescriptorCodeValue`
    , `st`.`Ethnicity` AS `Ethnicity`
    , `st`.`Race_AmericanIndianAlaskanNative` AS `Race_AmericanIndianAlaskanNative`
    , `st`.`Race_Asian` AS `Race_Asian`
    , `st`.`Race_BlackAfricaAmerican` AS `Race_BlackAfricaAmerican`
    , `st`.`Race_NativeHawaiianPacificIslander` AS `Race_NativeHawaiianPacificIslander`
    , `st`.`Race_White` AS `Race_White`
    , `st`.`Race_ChooseNotToRespond` AS `Race_ChooseNotToRespond`
    , `st`.`Race_Other` AS `Race_Other`
    , `st`.`DisabilityStatusDescriptorCodeValue` AS `DisabilityStatusDescriptorCodeValue`
    , `st`.`EconomicallyDisadvantageDescriptorCodeValue` AS `EconomicallyDisadvantageDescriptorCodeValue`
    , `st`.`ELLStatusDescriptorCodeValue` AS `ELLStatusDescriptorCodeValue`
    , `st`.`MigrantDescriptorCodeValue` AS `MigrantDescriptorCodeValue`
    , `st`.`HomelessDescriptorCodeValue` AS `HomelessDescriptorCodeValue`
    , `st`.`FosterDescriptorCodeValue` AS `FosterDescriptorCodeValue`
    , `st`.`F504DescriptorCodeValue` AS `F504DescriptorCodeValue`
    , `st`.`ContactInfoFirstName` AS `ContactInfoFirstName`
    , `st`.`ContactInfoLastSurname` AS `ContactInfoLastSurname`
    , `st`.`ContactInfoRelationToStudent` AS `ContactInfoRelationToStudent`
    , `st`.`ContactInfoCellPhoneNumber` AS `ContactInfoCellPhoneNumber`
    , `st`.`ContactInfoElectronicMailAddress` AS `ContactInfoElectronicMailAddress`
    ,
    (
        CASE WHEN (`ev`.`StudentElectronicMailAddress` IS NOT NULL) THEN 'Engaged today'
        ELSE 'Not Engaged today' END) AS `LoggedToday`
FROM
    (`db_9aa830_learnev`.`studentinformation` `st`
LEFT JOIN `db_9aa830_learnev`.`studentengagementreport_todaysengagement` `ev` ON
    ((`st`.`IdentityElectronicMailAddress` = `ev`.`StudentElectronicMailAddress`)))
    ;



CREATE OR REPLACE
ALGORITHM = UNDEFINED VIEW `db_9aa830_learnev`.`studentengagementreport_lastengagement` AS
SELECT
    `lg`.`StudentElectronicMailAddress` AS `StudentElectronicMailAddress`
    , MAX(`lg`.`UTCStartDate`) AS `DateLastEngagement`
    ,(TO_DAYS(CURDATE()) - TO_DAYS(MAX(`lg`.`UTCStartDate`))) AS `DaysSinceLastEngagement`
FROM
    `db_9aa830_learnev`.`studentlearningeventlog` `lg`
GROUP BY
    `lg`.`StudentElectronicMailAddress`
    ;