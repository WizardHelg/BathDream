using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class OrderMaterialToPaymentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderMaterialId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderMaterialId",
                table: "Payments",
                column: "OrderMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_OrderMaterials_OrderMaterialId",
                table: "Payments",
                column: "OrderMaterialId",
                principalTable: "OrderMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_OrderMaterials_OrderMaterialId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderMaterialId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderMaterialId",
                table: "Payments");
        }
    }
}
