using AdminPanelProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminPanelProject.Services
{
    public class UserClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimsService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Kullanıcının "IsAdmin" claim'ini kontrol eden metod
        public async Task<bool> IsAdminClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return false;

            // Kullanıcının claim'lerini al
            var userClaims = await _userManager.GetClaimsAsync(user);

            // Kullanıcının "IsAdmin" claim'ini kontrol et
            var isAdmin = userClaims.Any(c => c.Type == "IsAdmin" && c.Value == "true");

            return isAdmin;
        }
    }
}
