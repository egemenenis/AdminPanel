using AdminPanelProject.Models;
using System.Threading.Tasks;

namespace AdminPanelProject.Services.AccountService
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> FindUserByUsernameAsync(string username);
        Task<bool> SignInAsync(ApplicationUser user, string password);
        Task SignOutAsync();
    }
}
