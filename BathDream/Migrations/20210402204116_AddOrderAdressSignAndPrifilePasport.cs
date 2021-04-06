using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class AddOrderAdressSignAndPrifilePasport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pasport_Address",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pasport_Date",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pasport_Issued",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pasport_Number",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pasport_Serial",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectAdress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Signed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pasport_Address",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Pasport_Date",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Pasport_Issued",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Pasport_Number",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Pasport_Serial",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ObjectAdress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Signed",
                table: "Orders");
        }
    }
}
