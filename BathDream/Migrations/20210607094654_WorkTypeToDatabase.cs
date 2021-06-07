using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class WorkTypeToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkTypeId",
                table: "WorkPrices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPrices_WorkTypeId",
                table: "WorkPrices",
                column: "WorkTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkPrices_WorkTypes_WorkTypeId",
                table: "WorkPrices",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkPrices_WorkTypes_WorkTypeId",
                table: "WorkPrices");

            migrationBuilder.DropTable(
                name: "WorkTypes");

            migrationBuilder.DropIndex(
                name: "IX_WorkPrices_WorkTypeId",
                table: "WorkPrices");

            migrationBuilder.DropColumn(
                name: "WorkTypeId",
                table: "WorkPrices");
        }
    }
}
