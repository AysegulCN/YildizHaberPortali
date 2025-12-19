using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YildizHaberPortali.Contracts;
using YildizHaberPortali.Data;
using YildizHaberPortali.Repositories;
using YildizHaberPortali.Models;
using YildizHaberPortali.Hubs; // NewsHub için namespace

var builder = WebApplication.CreateBuilder(args);

// 1. VERÝTABANI BAÐLANTISI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Baðlantý cümlesi bulunamadý.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. IDENTITY (KULLANICI & ROL) AYARLARI
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 3. REPOSITORY & SIGNALR KAYITLARI (Sýralama Önemli)
builder.Services.AddSignalR(); // SignalR servisi burada kaydedilmeli
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. VERÝ TABANI TOHUMLAMA (SEED DATA) - Uygulama Baþlamadan Önce!
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Senkron yerine asenkron bekletme yapýyoruz
        await SeedData(roleManager, userManager, context);
    }
    catch (Exception ex)
    {
        // Hata durumunda loglama yapýlabilir
        Console.WriteLine("Seed iþlemi sýrasýnda hata: " + ex.Message);
    }
}

// 5. ARA KATMANLAR (MIDDLEWARE)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
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

// 6. YÖNLENDÝRMELER (ROUTES)
app.MapRazorPages();
app.MapHub<NewsHub>("/newsHub"); // SignalR Hub baðlantýsý

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// UYGULAMAYI ÇALIÞTIR (Bu satýrdan sonra kod yazýlmaz!)
app.Run();

// --- YARDIMCI METODLAR (SADECE TANIMLAR) ---

async Task SeedData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ApplicationDbContext context)
{
    // Kategori Tohumlama
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

    // Buraya Rol ve Kullanýcý tohumlama kodlarýný da ekleyebilirsin
}