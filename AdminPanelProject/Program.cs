using AdminPanelProject.Data;
using AdminPanelProject.Models;
using AdminPanelProject.Services;
using AdminPanelProject.Services.Repository.AccountService;
using AdminPanelProject.Services.Repository.ProductService;
using AdminPanelProject.Services.Repository.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// IHttpContextAccessor servisini ekleyin
builder.Services.AddHttpContextAccessor();

// Session'� yap�land�r�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum s�resi
    options.Cookie.HttpOnly = true;                  // G�venlik amac�yla HttpOnly olarak ayarland�
    options.Cookie.IsEssential = true;               // GDPR uyumlulu�u i�in gerekli
});

// SQL Server veritaban� ba�lant�s� ekleyin
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity servislerini ekleyin
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authorization filter servisini ekleyin
builder.Services.AddScoped<AuthorizationFilter>();

// MVC (Controller ve View) deste�i ekleyin
builder.Services.AddControllersWithViews();

// Repository servislerini ekleyin
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// UserClaimsService ve RoleClaimsService servislerini ekleyin
builder.Services.AddScoped<UserClaimsService>();
builder.Services.AddScoped<RoleClaimsService>();

var app = builder.Build();

// Hata y�netimi ve g�venlik i�in gerekli middleware'leri yap�land�r�n
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS y�nlendirmesi ve statik dosya eri�imi
app.UseHttpsRedirection();
app.UseStaticFiles();

// Session middleware'ini ekleyin
app.UseSession();

// Routing middleware'ini ekleyin
app.UseRouting();

// Authentication ve Authorization middleware'lerini ekleyin
app.UseAuthentication();
app.UseAuthorization();

// Veri ba�latma i�lemi (SeedData)
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
var signInManager = services.GetRequiredService<SignInManager<ApplicationUser>>();

await SeedData.Initialize(services, userManager, roleManager, signInManager);

// Route yap�land�rmas�n� ekleyin
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamay� ba�lat�n
app.Run();
