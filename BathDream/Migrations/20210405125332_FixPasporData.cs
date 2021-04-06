using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class FixPasporData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pasport_Date",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "Pasport_Serial",
                table: "UserProfiles",
                newName: "PasportSerial");

            migrationBuilder.RenameColumn(
                name: "Pasport_Number",
                table: "UserProfiles",
                newName: "PasportNumber");

            migrationBuilder.RenameColumn(
                name: "Pasport_Issued",
                table: "UserProfiles",
                newName: "PasportIssued");

            migrationBuilder.RenameColumn(
                name: "Pasport_Address",
                table: "UserProfiles",
                newName: "PasportAddress");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasportDate",
                table: "UserProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasportDate",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "PasportSerial",
                table: "UserProfiles",
                newName: "Pasport_Serial");

            migrationBuilder.RenameColumn(
                name: "PasportNumber",
                table: "UserProfiles",
                newName: "Pasport_Number");

            migrationBuilder.RenameColumn(
                name: "PasportIssued",
                table: "UserProfiles",
                newName: "Pasport_Issued");

            migrationBuilder.RenameColumn(
                name: "PasportAddress",
                table: "UserProfiles",
                newName: "Pasport_Address");

            migrationBuilder.AddColumn<DateTime>(
                name: "Pasport_Date",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);
        }
    }
}
