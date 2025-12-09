using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Models; // Modelleri import edin

namespace YildizHaberPortali.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; } // News tablosu
        public DbSet<Category> Categories { get; set; } // Categories tablosu
    }
}