using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı ve Identity yapılandırması
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Uygulama çerez yapılandırması
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Giriş yapılmamışsa yönlendirme
    options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisiz erişim için yönlendirme
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false; // Şifrelerde rakam zorunluluğunu kaldırır
    options.Password.RequiredLength = 3;  // Minimum şifre uzunluğunu 3'e indirir
    options.Password.RequireNonAlphanumeric = false; // Özel karakter gereksinimini kaldırır
    options.Password.RequireUppercase = false; // Büyük harf gereksinimini kaldırır
    options.Password.RequireLowercase = false; // Küçük harf gereksinimini kaldırır
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MyAppCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);

    // Çerez sürümü (her derlemede farklı bir sürüm kullanılırsa eski çerezler geçersiz olur)
    options.Cookie.Name += Guid.NewGuid().ToString();

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// MVC ve Razor Pages
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    context.Database.Migrate(); // Migration'ları otomatik olarak uygula

    // Seed işlemini çağırın
    await SeedUsersAndRoles(userManager, roleManager,context);
}

// Middleware'ler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Kimlik doğrulama middleware'i
app.UseAuthorization(); // Yetkilendirme middleware'i

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task SeedUsersAndRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
{
    try
    {
        Console.WriteLine("Seeding işlemi başladı.");

        // Rolleri oluştur
        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                Console.WriteLine($"{role} rolü oluşturuluyor...");
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"{role} rolü eklendi.");
            }
        }

        // Admin kullanıcıları ekle
        var adminEmails = new[] { "Y245012016@sakarya.edu.tr" };
        foreach (var adminEmail in adminEmails)
        {
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                Console.WriteLine($"Admin kullanıcısı oluşturuluyor: {adminEmail}");
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "sau"); // Varsayılan şifre
                if (result.Succeeded)
                {
                    Console.WriteLine($"Admin kullanıcısı eklendi: {adminEmail}");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    Console.WriteLine($"Admin kullanıcısı eklenirken hata oluştu: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Normal kullanıcıları ekle
        var userEmails = new[] { "TarikKartalBarber@sakarya.edu.tr", "TarikKartalHairDesigner@sakarya.edu.tr", "TarikKartal@sakarya.edu.tr" };
        foreach (var userEmail in userEmails)
        {
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                Console.WriteLine($"Normal kullanıcı oluşturuluyor: {userEmail}");
                var normalUser = new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(normalUser, "sau"); // Varsayılan şifre
                if (result.Succeeded)
                {
                    Console.WriteLine($"Kullanıcı eklendi: {userEmail}");
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
                else
                {
                    Console.WriteLine($"Kullanıcı eklenirken hata oluştu: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        if (!context.Skills.Any())
        {
            var skills = new List<Skill>
            {
                new Skill { Name = "Saç Kesimi" },
                new Skill { Name = "Saç Boyama" },
                new Skill { Name = "Kırık Alma" },
                new Skill { Name = "Saç Şekillendirme" },
                new Skill { Name = "Sakal Tıraşı" },
                new Skill { Name = "Cilt Bakımı" },
                new Skill { Name = "Keratin Bakımı" },
                new Skill { Name = "Perma" },
                new Skill { Name = "Kaş Tasarımı" },
                new Skill { Name = "Fön Çekme" }
            };

            context.Skills.AddRange(skills);
            context.SaveChanges();
        }

        Console.WriteLine("Seeding işlemi tamamlandı.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding sırasında hata oluştu: {ex.Message}");
    }
}
