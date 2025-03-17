using AdminPanelProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminPanelProject.Services.Repository.AccountService
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Kullanıcıyı kullanıcı adıyla bulma
        public async Task<ApplicationUser> FindUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        // Kullanıcıyı giriş yapmak
        public async Task<bool> SignInAsync(ApplicationUser user, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            return result.Succeeded;
        }

        // Kullanıcıyı çıkış yapmak
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // Kullanıcının rollerini almak
        public async Task<IList<string>> GetRolesForUserAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
