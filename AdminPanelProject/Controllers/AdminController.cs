using AdminPanelProject.Models;
using AdminPanelProject.Services;
using AdminPanelProject.Services.Repository.ProductService;
using AdminPanelProject.Services.Repository.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminPanelProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserClaimsService _userClaimsService;
        private readonly RoleClaimsService _roleClaimsService;

        public AdminController(IProductRepository productRepository, IUserRepository userRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
               UserClaimsService userClaimsService, RoleClaimsService roleClaimsService)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userClaimsService = userClaimsService;
            _roleClaimsService = roleClaimsService;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;

            if (username != null)
            {
                // username değişkenini güvenle kullanabilirsiniz
                // Örneğin: HttpContext.Session.SetString("Username", username);
            }
            else
            {
                // Kullanıcı giriş yapmamışsa yapılacak işlemler
                // Örneğin, varsayılan bir değer ayarlayabilirsiniz
                HttpContext.Session.SetString("Username", "Unknown User");
            }


            ViewData["UserId"] = userId;
            ViewData["Username"] = username;

            var products = await _productRepository.GetAllProductsAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizationFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {

                await _productRepository.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [ServiceFilter(typeof(AuthorizationFilter))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [ServiceFilter(typeof(AuthorizationFilter))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productRepository.UpdateProductAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _productRepository.GetProductByIdAsync(product.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [ServiceFilter(typeof(AuthorizationFilter))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [ServiceFilter(typeof(AuthorizationFilter))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            await _productRepository.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }


        [ServiceFilter(typeof(AuthorizationFilter))]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            // Giriş yapan kullanıcının ID'sini almak
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Kullanıcıyı ID ile bulmak
            if (string.IsNullOrEmpty(userId))
            {
                // userId null veya boş ise hata işlemi yapılabilir
                // Örneğin, bir hata mesajı gösterebilirsiniz:
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            // Eğer userId geçerli ise, işlemi gerçekleştirebilirsiniz
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