using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YildizHaberPortali.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryToNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "News");
        }
    }
}
