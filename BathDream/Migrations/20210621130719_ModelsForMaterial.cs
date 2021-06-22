using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class ModelsForMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_WorkPrices_WorkPriceId",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "WorkPriceId",
                table: "Works",
                newName: "WorkTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Works_WorkPriceId",
                table: "Works",
                newName: "IX_Works_WorkTypeId");

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

            migrationBuilder.CreateTable(
                name: "MaterialPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    StatusPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMaterials_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    OrderMaterialId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_OrderMaterials_OrderMaterialId",
                        column: x => x.OrderMaterialId,
                        principalTable: "OrderMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_OrderMaterialId",
                table: "Materials",
                column: "OrderMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMaterials_OrderId",
                table: "OrderMaterials",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_WorkTypes_WorkTypeId",
                table: "Works",
                column: "WorkTypeId",
                principalTable: "WorkTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_WorkTypes_WorkTypeId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "MaterialPrices");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "OrderMaterials");

            migrationBuilder.DropColumn(
                name: "InnerName",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "WorkTypeId",
                table: "Works",
                newName: "WorkPriceId");

            migrationBuilder.RenameIndex(
                name: "IX_Works_WorkTypeId",
                table: "Works",
                newName: "IX_Works_WorkPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_WorkPrices_WorkPriceId",
                table: "Works",
                column: "WorkPriceId",
                principalTable: "WorkPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
