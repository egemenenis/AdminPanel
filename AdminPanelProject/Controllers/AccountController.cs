using AdminPanelProject.Models;
using AdminPanelProject.Services.Repository.AccountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminPanelProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.FindUserByUsernameAsync(model.Username);
                if (user != null)
                {
                    var signInSucceeded = await _accountRepository.SignInAsync(user, model.Password);
                    if (signInSucceeded)
                    {
                        // Kullanıcının rolünü kontrol et
                        var roles = await _accountRepository.GetRolesForUserAsync(user);

                        // Admin rolünde mi diye kontrol et
                        if (roles.Contains("Admin"))
                        {
                            TempData["Message"] = "Başarılı giriş!";
                            TempData["RedirectUrl"] = Url.Action("Index", "Admin"); // Admin sayfasına yönlendirme
                            return RedirectToAction("LoginResult");
                        }
                        // User rolünde mi diye kontrol et
                        else if (roles.Contains("User"))
                        {
                            TempData["Message"] = "Başarılı giriş!";
                            TempData["RedirectUrl"] = Url.Action("Index", "User"); // User sayfasına yönlendirme
                            return RedirectToAction("LoginResult");
                        }
                        else
                        {
                            // Diğer roller için bir işlem yapılabilir
                            TempData["Message"] = "Başarılı giriş!";
                            TempData["RedirectUrl"] = Url.Action("Index", "Home"); // Varsayılan sayfa
                            return RedirectToAction("LoginResult");
                        }
                    }
                    // Hatalı giriş durumunda TempData'ya mesaj ekleyin
                    TempData["Message"] = "Hatalı giriş, lütfen tekrar deneyin!";
                    TempData["RedirectUrl"] = Url.Action("Login", "Account"); // Giriş sayfasına yönlendirme
                    return RedirectToAction("LoginResult");
                }
                else
                {
                    TempData["Message"] = "Kullanıcı bulunamadı!";
                    TempData["RedirectUrl"] = Url.Action("Login", "Account"); // Giriş sayfasına yönlendirme
                    return RedirectToAction("LoginResult");
                }
            }
            return View(model);
        }


        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountRepository.SignOutAsync();
            TempData["Message"] = "Oturum kapatıldı!";
            TempData["RedirectUrl"] = Url.Action("Index", "Home"); // Logout sonrası yönlendirme
            return RedirectToAction("LoginResult");
        }

        // Login sonrası sonuç mesajlarını gösterme
        public IActionResult LoginResult()
        {
            return View();
        }


        // Erişim engellendiği durumda buraya yönlendirilir.
        [AllowAnonymous]  // Bu sayfa herkese açık olmalı
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
