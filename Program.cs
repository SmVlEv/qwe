using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Data;
using UnityAssetStore.Models.Identity;
using UnityAssetStore.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к SQL Server через EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity: используем ApplicationUser и IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. Добавляем Session
builder.Services.AddDistributedMemoryCache(); // Требуется для Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".UnityAssetStore.Cart";
    options.Cookie.IsEssential = true;
});

// 4. MVC
builder.Services.AddControllersWithViews();

// 5. Регистрация сервисов
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();


// 6. Добавь Razor Pages (если используешь Identity UI)
builder.Services.AddRazorPages();

var app = builder.Build();

// Конфигурация middleware

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
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// --- Создание ролей и админа ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Создание ролей
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Создание администратора
    var adminUser = await userManager.FindByNameAsync("admin");

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@example.com",
            FirstName = "Иван",
            LastName = "Админов"
        };

        await userManager.CreateAsync(adminUser, "A@dm1nPassw0rd");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Маршруты
// Маршрут для Area "Admin" (должен быть выше дефолтного)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();