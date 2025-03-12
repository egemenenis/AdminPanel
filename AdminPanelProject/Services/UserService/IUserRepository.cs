using AdminPanelProject.Models;

namespace AdminPanelProject.Repositories
{
    public interface IUserRepository
    {
        Task<ProfileViewModel> GetUserProfileAsync(string userId);
    }
}
