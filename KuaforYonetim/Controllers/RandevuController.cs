using Microsoft.AspNetCore.Mvc;

namespace KuaforYonetim.Controllers
{
    public class RandevuController : Controller
    {
        public IActionResult Index()
        {
            // Randevu formu için sayfa döndürülüyor
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(string MüşteriAdı, string MüşteriTelefon, int HizmetID, DateTime TarihSaat)
        {
            // Randevu kaydı burada yapılacak
            TempData["Mesaj"] = "Randevu başarıyla alındı!";
            return RedirectToAction("Index");
        }
    }
}
