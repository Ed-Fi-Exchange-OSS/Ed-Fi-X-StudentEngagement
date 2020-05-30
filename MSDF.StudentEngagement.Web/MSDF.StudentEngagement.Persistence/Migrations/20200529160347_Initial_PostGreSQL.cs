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
CREATE OR REPLACE VIEW public.StudentEngagementReport
AS SELECT st.""Id"",
    st.""StudentUSI"",
    st.""StudentUniqueId"",
    st.""StudentStateIdentificationCode"",
    st.""IdentityElectronicMailAddress"",
    st.""DeviceId"",
    st.""LocalEducationAgencyName"",
    st.""SchoolName"",
    st.""SchoolYear"",
    st.""SchoolCurrentGradeLevelDescriptorCodeValue"",
    st.""SchoolTypeDescriptorCodeValue"",
    st.""ExitWithdrawalDate"",
    st.""FirstName"",
    st.""MiddleName"",
    st.""LastSurname"",
    concat(st.""LastSurname"", ' ', st.""FirstName"", COALESCE(concat(' ', st.""MiddleName""), ''::text)) AS ""StudentFullNameLFM"",
    st.""BirthDate"",
    st.""BirthSexDescriptorCodeValue"",
    st.""Ethnicity"",
    st.""Race_AmericanIndianAlaskanNative"",
    st.""Race_Asian"",
    st.""Race_BlackAfricaAmerican"",
    st.""Race_NativeHawaiianPacificIslander"",
    st.""Race_White"",
    st.""Race_ChooseNotToRespond"",
    st.""Race_Other"",
    st.""DisabilityStatusDescriptorCodeValue"",
    st.""EconomicallyDisadvantageDescriptorCodeValue"",
    st.""ELLStatusDescriptorCodeValue"",
    st.""MigrantDescriptorCodeValue"",
    st.""HomelessDescriptorCodeValue"",
    st.""FosterDescriptorCodeValue"",
    st.""F504DescriptorCodeValue"",
    st.""ContactInfoFirstName"",
    st.""ContactInfoLastSurname"",
    st.""ContactInfoRelationToStudent"",
    st.""ContactInfoCellPhoneNumber"",
    st.""ContactInfoElectronicMailAddress"",
    concat(st.""ContactInfoLastSurname"", ', ', st.""ContactInfoFirstName"", '(', st.""ContactInfoRelationToStudent"", ') - ', st.""ContactInfoCellPhoneNumber"") AS ""ContactInfo"",
        CASE
            WHEN te.""StudentElectronicMailAddress"" IS NOT NULL THEN 'Engaged today'::text
            ELSE 'Not Engaged today'::text
        END AS ""LoggedToday"",
    le.""DateLastEngagement"",
    le.""DaysSinceLastEngagement""
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
