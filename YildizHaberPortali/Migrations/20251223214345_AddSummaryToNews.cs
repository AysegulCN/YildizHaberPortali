using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YildizHaberPortali.Migrations
{
    public partial class AddSummaryToNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "News");
        }
    }
}
