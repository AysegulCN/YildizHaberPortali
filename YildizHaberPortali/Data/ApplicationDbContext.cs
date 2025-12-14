using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // GEREKLİ
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Models;


namespace YildizHaberPortali.Data
{
    //!!! IdentityDbContext'ten miras almalı !!!
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSet'ler de burada olmalı (Category, News, Comment)
        public DbSet<Category> Categories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}