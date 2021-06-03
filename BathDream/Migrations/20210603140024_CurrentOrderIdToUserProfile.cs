using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class CurrentOrderIdToUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentOrderId",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentOrderId",
                table: "UserProfiles");
        }
    }
}
