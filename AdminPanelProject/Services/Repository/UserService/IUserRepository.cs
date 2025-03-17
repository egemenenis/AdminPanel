using AdminPanelProject.Models;

namespace AdminPanelProject.Services.Repository.UserService
{
    public interface IUserRepository
    {
        Task<ProfileViewModel> GetUserProfileAsync(string userId);
    }
}
