// Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Models;

namespace YildizHaberPortali.Data
{
    // DbContext'ten türetildiğinden emin olun
    public class ApplicationDbContext : DbContext
    {
        // -----------------------------------------------------------------
        // HATA ÇÖZÜMÜ BURADA: Doğru Constructor Tanımı
        // Program.cs'teki AddDbContext ayarlarını alması için zorunludur.
        // -----------------------------------------------------------------
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Bu gövde boş kalabilir
        }
        // -----------------------------------------------------------------

        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Bu metot, migration yapmaya çalıştığınızda da kullanılacaktır.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Slug benzersiz olmalı kuralı (daha önce eklediğimiz)
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Eğer isterseniz, Name alanını da unique yapabilirsiniz.
        }
    }
}