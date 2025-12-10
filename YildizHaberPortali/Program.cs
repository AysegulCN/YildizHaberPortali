using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts; 
using YildizHaberPortali.Data;
using YildizHaberPortali.Repositories; 


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LoginPath = "/Account/Login";
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();




builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();



builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Scopes (kapsamlar) oluþturulmalý, çünkü Identity servisleri scope'ludur.
    using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Rolleri ve Admin kullanýcýyý oluþtur
    SeedData(roleManager, userManager).Wait();
}
}

// Configure the HTTP request pipeline.
// ... (app.UseExceptionHandler, app.UseHttpsRedirection, vs.) ...

app.Run(); // Uygulamanýn en son satýrý

// --------------------------------------------------------------------------------
// YENÝ METOT: Seed Data
// --------------------------------------------------------------------------------

async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    // 1. "Admin" Rolünü Kontrol Et ve Oluþtur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2. "Editor" Rolünü Kontrol Et ve Oluþtur (Opsiyonel)
    if (!await roleManager.RoleExistsAsync("Editor"))
    {
        await roleManager.CreateAsync(new IdentityRole("Editor"));
    }

    // 3. Ýlk Admin Kullanýcýsýný Oluþtur
    if (await userManager.FindByEmailAsync("admin@yildizhaber.com") == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = "admin@yildizhaber.com",
            Email = "admin@yildizhaber.com",
            EmailConfirmed = true // Email onayýný atla
        };

        // Kullanýcýyý oluþtur
        var result = await userManager.CreateAsync(adminUser, "P@ssword123"); // Güvenli bir þifre belirleyin!

        // Kullanýcýya Admin Rolünü Ata
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}