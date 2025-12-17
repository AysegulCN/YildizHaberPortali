using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Repositories;
using YildizHaberPortali.Models; // Modelleri kullanmak için ekledik (SeedData için gerekli olabilir)

var builder = WebApplication.CreateBuilder(args);

// Connection string'i kontrol etme ve alma
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Hesap doðrulama zorunluluðunu kaldýrýyoruz (Hýzlý giriþ için)
    options.SignIn.RequireConfirmedAccount = false;

    // --- ÞÝFRE AYARLARI (Zorunluluklarý Kaldýrma) ---
    options.Password.RequireDigit = false;           // Sayý zorunluluðu yok
    options.Password.RequireLowercase = false;       // Küçük harf zorunluluðu yok
    options.Password.RequireUppercase = false;       // Büyük harf zorunluluðu yok
    options.Password.RequireNonAlphanumeric = false; // Sembol (!, *, ?) zorunluluðu yok
    options.Password.RequiredLength = 3;             // En az 3 karakter olsun yeter (Ýstersen 6 yap)
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        SeedData(roleManager, userManager).Wait();
    }
}

// 4. Uygulama Konfigürasyonu
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Home/ErrorPage", "?code={0}");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=News}/{action=Index}/{id?}");

app.Run();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        await SeedData(roleManager, userManager);
        await SeedCategories(app.Services);
    }
}


async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    // ... (Mevcut Rol ve Kullanýcý Ekleme Kodlarý) ...

    // YENÝ EKLEME: Kategorileri Ekleme
    await SeedCategories(app.Services); // <<< app.Services'i kullanmak için bu metodu Program.cs içinde çaðýrýyoruz.
}

async Task SeedCategories(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Eðer kategori yoksa, ekle
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
                new Category { Name = "Kadýn", Slug = "kadin" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
}

 