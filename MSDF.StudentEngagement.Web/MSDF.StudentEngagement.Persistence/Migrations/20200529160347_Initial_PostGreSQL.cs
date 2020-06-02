using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MSDF.StudentEngagement.Persistence.Migrations
{
    public partial class Initial_PostGreSQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningApp",
                columns: table => new
                {
                    LearningAppIdentifier = table.Column<string>(maxLength: 60, nullable: false),
                    Namespace = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Website = table.Column<string>(maxLength: 255, nullable: true),
                    AppUrl = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningApp", x => x.LearningAppIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "StudentInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUSI = table.Column<int>(nullable: false),
                    StudentUniqueId = table.Column<string>(maxLength: 32, nullable: true),
                    StudentStateIdentificationCode = table.Column<string>(maxLength: 60, nullable: true),
                    IdentityElectronicMailAddress = table.Column<string>(maxLength: 60, nullable: true),
                    DeviceId = table.Column<string>(maxLength: 100, nullable: true),
                    LocalEducationAgencyName = table.Column<string>(maxLength: 75, nullable: true),
                    SchoolName = table.Column<string>(maxLength: 75, nullable: true),
                    SchoolYear = table.Column<string>(maxLength: 15, nullable: true),
                    SchoolCurrentGradeLevelDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    SchoolTypeDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    ExitWithdrawalDate = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 75, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 75, nullable: true),
                    LastSurname = table.Column<string>(maxLength: 75, nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    BirthSexDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    Ethnicity = table.Column<string>(maxLength: 25, nullable: true),
                    Race_AmericanIndianAlaskanNative = table.Column<bool>(nullable: true),
                    Race_Asian = table.Column<bool>(nullable: true),
                    Race_BlackAfricaAmerican = table.Column<bool>(nullable: true),
                    Race_NativeHawaiianPacificIslander = table.Column<bool>(nullable: true),
                    Race_White = table.Column<bool>(nullable: true),
                    Race_ChooseNotToRespond = table.Column<bool>(nullable: true),
                    Race_Other = table.Column<bool>(nullable: true),
                    DisabilityStatusDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    EconomicallyDisadvantageDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    ELLStatusDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    MigrantDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    HomelessDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    FosterDescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    F504DescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true),
                    ContactInfoFirstName = table.Column<string>(maxLength: 75, nullable: true),
                    ContactInfoLastSurname = table.Column<string>(maxLength: 75, nullable: true),
                    ContactInfoRelationToStudent = table.Column<string>(maxLength: 75, nullable: true),
                    ContactInfoCellPhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    ContactInfoElectronicMailAddress = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentLearningEventLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentUSI = table.Column<int>(nullable: false),
                    StudentUniqueId = table.Column<string>(maxLength: 32, nullable: true),
                    DeviceId = table.Column<string>(maxLength: 32, nullable: true),
                    StudentElectronicMailAddress = table.Column<string>(maxLength: 128, nullable: true),
                    IPAddress = table.Column<string>(maxLength: 15, nullable: true),
                    ReffererUrl = table.Column<string>(maxLength: 1024, nullable: true),
                    LeaningAppUrl = table.Column<string>(maxLength: 1024, nullable: true),
                    UTCStartDate = table.Column<DateTime>(nullable: false),
                    UTCEndDate = table.Column<DateTime>(nullable: true),
                    TimeSpent = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLearningEventLog", x => x.Id);
                });


            SQL_CreateView(migrationBuilder);
        }

        private void SQL_CreateView(MigrationBuilder migrationBuilder)
        {

            string query = @"
CREATE OR REPLACE VIEW public.studentengagementreport
AS SELECT st.""Id"" AS id,
    st.""StudentUSI"" AS studentusi,
    st.""StudentUniqueId"" AS studentuniqueid,
    st.""StudentStateIdentificationCode"" AS studentstateidentificationcode,
    st.""IdentityElectronicMailAddress"" AS identityelectronicmailaddress,
    st.""DeviceId"" AS deviceid,
    st.""LocalEducationAgencyName"" AS localeducationagencyname,
    st.""SchoolName"" AS schoolname,
    st.""SchoolYear"" AS schoolyear,
    st.""SchoolCurrentGradeLevelDescriptorCodeValue"" AS schoolcurrentgradeleveldescriptorcodevalue,
    st.""SchoolTypeDescriptorCodeValue"" AS schooltypedescriptorcodevalue,
    st.""ExitWithdrawalDate"" AS exitwithdrawaldate,
    st.""FirstName"" AS firstname,
    st.""MiddleName"" AS middlename,
    st.""LastSurname"" AS lastsurname,
    concat(st.""LastSurname"", ' ', st.""FirstName"", COALESCE(concat(' ', st.""MiddleName""), ''::text)) AS studentfullnamelfm,
    st.""BirthDate"" AS birthdate,
    st.""BirthSexDescriptorCodeValue"" AS birthsexdescriptorcodevalue,
    st.""Ethnicity"",
    st.""Race_AmericanIndianAlaskanNative"" AS race_americanindianalaskannative,
    st.""Race_Asian"" AS race_asian,
    st.""Race_BlackAfricaAmerican"" AS race_blackafricaamerican,
    st.""Race_NativeHawaiianPacificIslander"" AS race_nativehawaiianpacificislander,
    st.""Race_White"" AS race_white,
    st.""Race_ChooseNotToRespond"" AS race_choosenottorespond,
    st.""Race_Other"" AS race_other,
    st.""DisabilityStatusDescriptorCodeValue"" AS disabilitystatusdescriptorcodevalue,
    st.""EconomicallyDisadvantageDescriptorCodeValue"" AS economicallydisadvantagedescriptorcodevalue,
    st.""ELLStatusDescriptorCodeValue"" AS ellstatusdescriptorcodevalue,
    st.""MigrantDescriptorCodeValue"" AS migrantdescriptorcodevalue,
    st.""HomelessDescriptorCodeValue"" AS homelessdescriptorcodevalue,
    st.""FosterDescriptorCodeValue"" AS fosterdescriptorcodevalue,
    st.""F504DescriptorCodeValue"" AS f504descriptorcodevalue,
    st.""ContactInfoFirstName"" AS contactinfofirstname,
    st.""ContactInfoLastSurname"" AS contactinfolastsurname,
    st.""ContactInfoRelationToStudent"" AS contactinforelationtostudent,
    st.""ContactInfoCellPhoneNumber"" AS contactinfocellphonenumber,
    st.""ContactInfoElectronicMailAddress"" AS contactinfoelectronicmailaddress,
    concat(st.""ContactInfoLastSurname"", ', ', st.""ContactInfoFirstName"", '(', st.""ContactInfoRelationToStudent"", ') - ', st.""ContactInfoCellPhoneNumber"") AS contactinfo,
        CASE
            WHEN te.""StudentElectronicMailAddress"" IS NOT NULL THEN 'Engaged Today'::text
            ELSE 'Not Engaged Today'::text
        END AS loggedtoday,
    le.""DateLastEngagement"" AS datelastengagement,
    le.""DaysSinceLastEngagement"" AS dayssincelastengagement
   FROM ""StudentInformation"" st
     LEFT JOIN(SELECT teurl.""StudentElectronicMailAddress"",
            count(0) AS ""URLsVisitedToday"",
            avg(teurl.""TimeSpent"") AS ""AVGTimeSpent""
           FROM(SELECT slel.""StudentElectronicMailAddress"",
                    slel.""LeaningAppUrl"",
                    slel.""UTCStartDate"",
                    max(slel.""UTCEndDate"") AS ""UTCEndDate"",
                    max(slel.""TimeSpent"") AS ""TimeSpent""
                   FROM ""StudentLearningEventLog"" slel
                  WHERE slel.""UTCStartDate"" > CURRENT_DATE
                  GROUP BY slel.""StudentElectronicMailAddress"", slel.""LeaningAppUrl"", slel.""UTCStartDate"") teurl
          GROUP BY teurl.""StudentElectronicMailAddress"") te ON st.""IdentityElectronicMailAddress""::text = te.""StudentElectronicMailAddress""::text
     LEFT JOIN(SELECT lg.""StudentElectronicMailAddress"",
            max(lg.""UTCStartDate"") AS ""DateLastEngagement"",
            date_part('day'::text, timezone('utc'::text, now()) - max(lg.""UTCStartDate"")) AS ""DaysSinceLastEngagement""
           FROM ""StudentLearningEventLog"" lg
          GROUP BY lg.""StudentElectronicMailAddress"") le ON st.""IdentityElectronicMailAddress""::text = le.""StudentElectronicMailAddress""::text;
            ";

            migrationBuilder.Sql(query);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningApp");

            migrationBuilder.DropTable(
                name: "StudentInformation");

            migrationBuilder.DropTable(
                name: "StudentLearningEventLog");
        }
    }
}
