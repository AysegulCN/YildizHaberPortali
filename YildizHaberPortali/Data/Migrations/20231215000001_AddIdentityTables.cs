using Microsoft.EntityFrameworkCore.Migrations;

namespace YildizHaberPortali.Data.Migrations
{
    public partial class AddIdentityTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AspNetUsers, AspNetRoles ve diğer tüm Identity tablolarının CREATE TABLE kodları buraya gelmelidir.
            // Bu kodlar, SSMS'te çalıştırdığımız devasa SQL sorgusundan kopyalanabilir.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tüm Identity tabloları için DropTable komutları buraya gelmelidir.
        }
    }
}