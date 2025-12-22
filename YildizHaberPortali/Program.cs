using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Repositories;
using YildizHaberPortali.Models;
using YildizHaberPortali.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. VERİTABANI BAĞLANTISI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Bağlantı cümlesi bulunamadı.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🚀 IDENTITY KAYDI (SADECE BİR KEZ VE BU ŞEKİLDE OLMALI)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddSignalR();

// REPOSITORY ENJEKSİYONLARI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 2. SEED DATA (ROL / KATEGORİ / ADMIN)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Seed işlemi hatası: " + ex.Message);
    }
}

app.UseStatusCodePagesWithReExecute("/Home/ErrorPage", "?code={0}");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // 🚀 ÖNEMLİ: Authorization'dan önce gelmeli!
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<NewsHub>("/newsHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();





// ===================
// SEED METODU
// ===================
async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ApplicationDbContext context)
{
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Name = "Gündem", Slug = "gundem" },
            new Category { Name = "Ekonomi", Slug = "ekonomi" },
            new Category { Name = "Spor", Slug = "spor" },
            new Category { Name = "Teknoloji", Slug = "teknoloji" },
            new Category { Name = "Dünya", Slug = "dunya" },
            new Category { Name = "Son Dakika", Slug = "son-dakika" },
            new Category { Name = "Kadın", Slug = "kadin" }
        };
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}