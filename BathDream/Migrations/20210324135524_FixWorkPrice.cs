using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class FixWorkPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkPrices_Estimates_EstimateId",
                table: "WorkPrices");

            migrationBuilder.DropIndex(
                name: "IX_WorkPrices_EstimateId",
                table: "WorkPrices");

            migrationBuilder.DropColumn(
                name: "EstimateId",
                table: "WorkPrices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimateId",
                table: "WorkPrices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkPrices_EstimateId",
                table: "WorkPrices",
                column: "EstimateId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkPrices_Estimates_EstimateId",
                table: "WorkPrices",
                column: "EstimateId",
                principalTable: "Estimates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
