using AdminPanelProject.Models;
using System.Threading.Tasks;

namespace AdminPanelProject.Services.Repository.AccountService
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> FindUserByUsernameAsync(string username);
        Task<bool> SignInAsync(ApplicationUser user, string password);
        Task SignOutAsync();
        Task<IList<string>> GetRolesForUserAsync(ApplicationUser user);
    }
}
