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

        // Index - Admin'deki �r�nlerin Home/index sayfas�nda g�sterilmesi
        public async Task<IActionResult> Index()
        {
            // Veritaban�ndaki t�m �r�nleri �ekiyoruz
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // Error - Hata sayfas�
        public IActionResult Error()
        {
            return View();
        }
    }
}
