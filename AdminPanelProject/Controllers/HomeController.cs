using AdminPanelProject.Data;
using AdminPanelProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index - Admin'deki ürünlerin Home/index sayfasýnda gösterilmesi
        public async Task<IActionResult> Index()
        {
            // Veritabanýndaki tüm ürünleri çekiyoruz
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Error - Hata sayfasý
        public IActionResult Error()
        {
            return View();
        }
    }
}
