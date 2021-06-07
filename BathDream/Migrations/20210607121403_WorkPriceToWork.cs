using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class WorkPriceToWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InnerName",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Works");

            migrationBuilder.AddColumn<int>(
                name: "WorkPriceId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Works_WorkPriceId",
                table: "Works",
                column: "WorkPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_WorkPrices_WorkPriceId",
                table: "Works",
                column: "WorkPriceId",
                principalTable: "WorkPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_WorkPrices_WorkPriceId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_WorkPriceId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "WorkPriceId",
                table: "Works");

            migrationBuilder.AddColumn<string>(
                name: "InnerName",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
