using Microsoft.EntityFrameworkCore.Migrations;

namespace YildizHaberPortali.Data.Migrations
{
    public partial class InitialCoreSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Slug = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Categories", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 150, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(name: "FK_News_Categories_CategoryId", column: x => x.CategoryId, principalTable: "Categories", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    NewsId = table.Column<int>(nullable: false),
                    AuthorName = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    CommentDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(name: "FK_Comments_News_NewsId", column: x => x.NewsId, principalTable: "News", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Comments");
            migrationBuilder.DropTable(name: "News");
            migrationBuilder.DropTable(name: "Categories");
        }
    }
}