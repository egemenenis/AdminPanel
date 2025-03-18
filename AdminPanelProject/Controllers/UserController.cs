using AdminPanelProject.Data;
using AdminPanelProject.Models;
using AdminPanelProject.Services.Repository.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdminPanelProject.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name ?? "UnknownName";


            ViewData["UserId"] = userId;
            ViewData["Username"] = username;

            var products = await _productRepository.GetAllProductsAsync();
            return View(products);
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            // Giriş yapan kullanıcının ID'sini almak
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcıyı ID ile bulmak
            if (string.IsNullOrEmpty(userId))
            {
                // Hata veya uygun bir işlem yapılabilir
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {
                return NotFound();
            }

            // Kullanıcı rolleri
            var roles = await _userManager.GetRolesAsync(user);

            // Kullanıcı bilgilerini bir ViewModel'e aktarıyoruz
            var model = new ProfileViewModel
            {
                Username = user.UserName ?? "DefaultUsername",
                Email = user.Email ?? "DefaultEmail",
                PhoneNumber = user.PhoneNumber ?? "DefaultPhoneNumber",
                Role = string.Join(", ", roles),
                AvatarUrl = user.AvatarUrl,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                City = user.City,
                Country = user.Country,
                Gender = user.Gender,
                PreferredLanguage = user.PreferredLanguage,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive,
                ReceiveNotifications = user.ReceiveNotifications
            };

            return View(model);
        }
    }
}
