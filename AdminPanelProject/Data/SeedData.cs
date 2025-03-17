using AdminPanelProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminPanelProject.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider,
                                            UserManager<ApplicationUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            SignInManager<ApplicationUser> signInManager)
        {
            // Roller tanımlanıyor
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin kullanıcısını ekle
            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");

            if (adminUser == null)
            {
                adminUser = new ApplicationUser() { UserName = "admin@admin.com", Email = "admin@admin.com" };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // Admin rolünü ata
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Admin rolüne CanViewProduct claim'ini ekle
                    var userRole = await roleManager.FindByNameAsync("Admin");
                    if (userRole != null)
                    {
                        var claim = new Claim("CanViewProduct", "true");
                        var claimResult = await roleManager.AddClaimAsync(userRole, claim);
                        if (!claimResult.Succeeded)
                        {
                            foreach (var error in claimResult.Errors)
                            {
                                Console.WriteLine(error.Description);
                            }
                        }
                    }

                    // Admin rolüne CanAccessAdminPanel claim'ini ekle
                    var adminRole = await roleManager.FindByNameAsync("Admin");
                    if (adminRole != null)
                    {
                        var claim = new Claim("CanAccessAdminPanel", "true");
                        var claimResult = await roleManager.AddClaimAsync(adminRole, claim);
                        if (!claimResult.Succeeded)
                        {
                            foreach (var error in claimResult.Errors)
                            {
                                Console.WriteLine(error.Description);
                            }
                        }
                    }

                    // Admin kullanıcıya claim ekle
                    var adminClaim = new Claim("IsAdmin", "true");
                    await userManager.AddClaimAsync(adminUser, adminClaim);

                    // Admin kullanıcısı için login verisi ekle
                    var loginInfo = new UserLoginInfo("Google", "admin-google-id", "Google");
                    await userManager.AddLoginAsync(adminUser, loginInfo);

                    // 2FA token'ı ile Admin kullanıcısına 2FA etkinleştir
                    await signInManager.RefreshSignInAsync(adminUser); // Sign-in işlemini tazeleyin
                    var token = await userManager.GenerateTwoFactorTokenAsync(adminUser, "Authenticator"); // 2FA token oluştur
                    Console.WriteLine($"Generated 2FA token for admin: {token}");

                    // Admin kullanıcıyı 2FA etkinleştir
                    await userManager.SetTwoFactorEnabledAsync(adminUser, true);
                }
            }

            // User kullanıcısını ekle (eğer yoksa)
            var userExist = await userManager.FindByEmailAsync("user@user.com");

            if (userExist == null)
            {
                userExist = new ApplicationUser() { UserName = "user@user.com", Email = "user@user.com" };
                var result = await userManager.CreateAsync(userExist, "User@123");

                if (result.Succeeded)
                {
                    // User rolünü ata
                    await userManager.AddToRoleAsync(userExist, "User");

                    // User rolüne CanViewProduct claim ekle
                    var userRole = await roleManager.FindByNameAsync("User");
                    if (userRole != null)
                    {
                        var claim = new Claim("CanViewProduct", "true");
                        var claimResult = await roleManager.AddClaimAsync(userRole, claim);
                        if (!claimResult.Succeeded)
                        {
                            foreach (var error in claimResult.Errors)
                            {
                                Console.WriteLine(error.Description);
                            }
                        }
                    }

                    // User kullanıcısına CanViewProduct claim ekle
                    var userClaim = new Claim("CanViewProduct", "true");
                    await userManager.AddClaimAsync(userExist, userClaim);

                    // User kullanıcısı için login verisi ekle
                    var userLoginInfo = new UserLoginInfo("Google", "user-google-id", "Google");
                    await userManager.AddLoginAsync(userExist, userLoginInfo);

                    // 2FA token'ı ile User kullanıcısına 2FA etkinleştir
                    await signInManager.RefreshSignInAsync(userExist); // Sign-in işlemini tazeleyin
                    var userToken = await userManager.GenerateTwoFactorTokenAsync(userExist, "Authenticator"); // 2FA token oluştur
                    Console.WriteLine($"Generated 2FA token for user: {userToken}");

                    // User kullanıcıyı 2FA etkinleştir
                    await userManager.SetTwoFactorEnabledAsync(userExist, true);
                }
            }
        }
    }
}
