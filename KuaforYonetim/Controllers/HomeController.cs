using KuaforYonetim.Data;
using KuaforYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaforYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            // Hizmet modelini alın (örnek)
            var hizmet = _context.Hizmetler.FirstOrDefault(h => h.HizmetID == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            // Salonlar listesini ViewData'ya ekle
            ViewData["Salonlar"] = _context.Salonlar.ToList();

            // Hizmet modelini View'a gönder
            return View(hizmet);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}