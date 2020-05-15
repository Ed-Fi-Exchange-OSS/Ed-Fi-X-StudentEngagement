using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MSDF.StudentEngagement.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentInformation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    ExitWithdrawalDate = table.Column<DateTime>(nullable: false),
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
                    F504DescriptorCodeValue = table.Column<string>(maxLength: 50, nullable: true)
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
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentUSI = table.Column<int>(nullable: false),
                    StudentUniqueId = table.Column<string>(maxLength: 32, nullable: true),
                    DeviceId = table.Column<string>(maxLength: 32, nullable: true),
                    StudentElectronicMailAddress = table.Column<string>(maxLength: 128, nullable: true),
                    IPAddress = table.Column<string>(maxLength: 15, nullable: true),
                    ReffererUrl = table.Column<string>(maxLength: 1024, nullable: true),
                    LeaningAppUrl = table.Column<string>(maxLength: 1024, nullable: true),
                    UTCDateTimeStart = table.Column<DateTime>(nullable: false),
                    UTCDateTimeEnd = table.Column<DateTime>(nullable: false),
                    DurationInSeconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLearningEventLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentInformation");

            migrationBuilder.DropTable(
                name: "StudentLearningEventLog");
        }
    }
}
