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

// 1. Veritabaný Baðlantýsý ve Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Servisleri
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 2. Repository/Servis Tanýmlamalarý (Tekrar edenleri sildik)
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// MVC ve Görünüm Servisleri
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. SeedData Çalýþtýrma (Sadece Development Ortamýnda)
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// En son Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=News}/{action=Index}/{id?}");


app.Run();

// SeedData Metodu (Bu kýsým zaten doðruydu)
async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    // ... (SeedData içeriði) ...
}