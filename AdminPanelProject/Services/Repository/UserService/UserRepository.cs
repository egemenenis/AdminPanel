﻿using AdminPanelProject.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminPanelProject.Services.Repository.UserService
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Kullanıcının profil bilgilerini almak için metod
        public async Task<ProfileViewModel> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new ProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
