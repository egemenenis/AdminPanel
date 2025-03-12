using AdminPanelProject.Models;
using AdminPanelProject.Services.AccountService;
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
                        TempData["Message"] = "Başarılı giriş!";
                        TempData["RedirectUrl"] = Url.Action("Index", "Admin"); // Yönlendirme URL'si
                        return RedirectToAction("LoginResult");
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
    }
}
