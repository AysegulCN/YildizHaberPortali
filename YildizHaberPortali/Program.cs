using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1. Connection String
// ----------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ----------------------------------------------------
// 2. SERVÝSLERÝ EKLE (Dependency Injection)
// ----------------------------------------------------

// Db Context (Sadece bir kez tanýmlanmalý)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Servisini Kurma (Sadece bir kez tanýmlanmalý)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Login sayfamýz Account/Login'de olacak (Login View ve Controller'ý oluþturduðumuzu varsayýyoruz)
    options.AccessDeniedPath = "/Account/AccessDenied"; // <<< Geri getirdik
    options.LoginPath = "/Account/Login";
})
    .AddRoles<IdentityRole>() // Rol Yönetimi için
    .AddEntityFrameworkStores<ApplicationDbContext>();


// Repository Kayýtlarý
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();

// Controller ve View'lar
builder.Services.AddControllersWithViews();

// ----------------------------------------------------
// 3. MIDDLEWARE PIPELINE'I OLUÞTUR
// ----------------------------------------------------

var app = builder.Build();

// Seed Data Ýþlemi (Admin ve Roller)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        SeedData(roleManager, userManager).Wait();
    }
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// SIRA KRÝTÝKTÝR: UseAuthentication önce, UseAuthorization sonra gelir.
app.UseAuthentication();
app.UseAuthorization();

// En son Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// --------------------------------------------------------------------------------
// YENÝ METOT: Seed Data (SeedData metodu uygulamanýn dýþýnda, dosyanýn alt kýsmýnda kalmalý)
// --------------------------------------------------------------------------------

async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    if (!await roleManager.RoleExistsAsync("Editor"))
    {
        await roleManager.CreateAsync(new IdentityRole("Editor"));
    }

    if (await userManager.FindByEmailAsync("admin@yildizhaber.com") == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = "admin@yildizhaber.com",
            Email = "admin@yildizhaber.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "P@ssword123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}