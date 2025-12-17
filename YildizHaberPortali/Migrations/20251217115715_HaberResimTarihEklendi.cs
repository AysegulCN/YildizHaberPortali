using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YildizHaberPortali.Migrations
{
    /// <inheritdoc />
    public partial class HaberResimTarihEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- DÜZELTİLDİ ---
            // Eski kodlarda 'CreateTable' vardı, hepsini sildik.
            // Sadece News tablosuna eksik sütunları ekliyoruz.

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            // Varsayılan tarih atadık ki eski haberler hata vermesin
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eğer işlemi geri alırsan, sadece eklediğin sütunları sil
            migrationBuilder.DropColumn(
                name: "Image",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "News");
        }
    }
}