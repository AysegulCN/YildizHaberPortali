using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YildizHaberPortali.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // İŞTE EKSİK OLAN PARÇA BU:
            // News tablosuna Description (Açıklama) sütunu ekliyoruz.
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri alırsak sütunu sil
            migrationBuilder.DropColumn(
                name: "Description",
                table: "News");
        }
    }
}