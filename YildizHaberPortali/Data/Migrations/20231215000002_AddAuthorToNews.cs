using Microsoft.EntityFrameworkCore.Migrations;

namespace YildizHaberPortali.Data.Migrations
{
    public partial class AddAuthorToNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Haber tablosuna Author sütununu ekle
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "News",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "News");
        }
    }
}