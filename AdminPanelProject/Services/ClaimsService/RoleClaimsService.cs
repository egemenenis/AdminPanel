using AdminPanelProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminPanelProject.Services
{
    public class RoleClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimsService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Kullanıcının rolüne ait "CanViewProduct" claim'ini kontrol eden metod
        public async Task<bool> CanViewProductClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            // Kullanıcının rollerini al
            var userRoles = await _userManager.GetRolesAsync(user);

            // Kullanıcı rollerini döngü ile kontrol et
            foreach (var roleName in userRoles)
            {
                // Rolü bul
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                // Rolün ID'sini al
                var roleId = role.Id;

                // AspNetRoleClaims tablosunda bu RoleId'ye sahip claim'leri al
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                // RoleId'ye sahip claim'lerde "CanViewProduct" claim'inin "true" olup olmadığını kontrol et
                var canViewProductClaim = roleClaims.FirstOrDefault(c => c.Type == "CanViewProduct" && c.Value == "true");

                if (canViewProductClaim != null)
                {
                    return true; // Eğer claim varsa, true döndür
                }
            }

            return false; // Eğer hiçbir rolde uygun claim bulunmazsa, false döndür
        }
    }
}
