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

// Session'ý yapýlandýrýn
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Oturum süresi
    options.Cookie.HttpOnly = true;                  // Güvenlik amacýyla HttpOnly olarak ayarlandý
    options.Cookie.IsEssential = true;               // GDPR uyumluluðu için gerekli
});

// SQL Server veritabaný baðlantýsý ekleyin
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity servislerini ekleyin
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Authorization filter servisini ekleyin
builder.Services.AddScoped<AuthorizationFilter>();

// MVC (Controller ve View) desteði ekleyin
builder.Services.AddControllersWithViews();

// Repository servislerini ekleyin
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// UserClaimsService ve RoleClaimsService servislerini ekleyin
builder.Services.AddScoped<UserClaimsService>();
builder.Services.AddScoped<RoleClaimsService>();

var app = builder.Build();

// Hata yönetimi ve güvenlik için gerekli middleware'leri yapýlandýrýn
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS yönlendirmesi ve statik dosya eriþimi
app.UseHttpsRedirection();
app.UseStaticFiles();

// Session middleware'ini ekleyin
app.UseSession();

// Routing middleware'ini ekleyin
app.UseRouting();

// Authentication ve Authorization middleware'lerini ekleyin
app.UseAuthentication();
app.UseAuthorization();

// Veri baþlatma iþlemi (SeedData)
var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
var signInManager = services.GetRequiredService<SignInManager<ApplicationUser>>();

await SeedData.Initialize(services, userManager, roleManager, signInManager);

// Route yapýlandýrmasýný ekleyin
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamayý baþlatýn
app.Run();
