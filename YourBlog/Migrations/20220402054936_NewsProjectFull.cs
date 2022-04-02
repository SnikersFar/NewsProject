using Microsoft.EntityFrameworkCore.Migrations;

namespace YourBlog.Migrations
{
    public partial class NewsProjectFull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_IsCategoryId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Articles_IsCategoryId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsCategoryId",
                table: "Articles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IsCategoryId",
                table: "Articles",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_IsCategoryId",
                table: "Articles",
                column: "IsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_IsCategoryId",
                table: "Articles",
                column: "IsCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
