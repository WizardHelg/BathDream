using Microsoft.EntityFrameworkCore.Migrations;

namespace BathDream.Migrations
{
    public partial class FilesAddToMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "FileItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FrendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileItem_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileItem_MessageId",
                table: "FileItem",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileItem");

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
