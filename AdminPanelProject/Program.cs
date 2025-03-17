using AdminPanelProject.Data;
using AdminPanelProject.Models;
using AdminPanelProject.Services;
using AdminPanelProject.Services.Repository.AccountService;
using AdminPanelProject.Services.Repository.ProductService;
using AdminPanelProject.Services.Repository.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<AuthorizationFilter>();
builder.Services.AddControllersWithViews(
    //options =>
    //{
    //    options.Filters.AddService<AuthorizationFilter>();
    //}
    );

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserClaimsService>();
builder.Services.AddScoped<RoleClaimsService>();


var app = builder.Build();

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

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
var signInManager = services.GetRequiredService<SignInManager<ApplicationUser>>();

await SeedData.Initialize(services, userManager, roleManager, signInManager);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
