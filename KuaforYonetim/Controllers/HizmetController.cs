using Microsoft.AspNetCore.Mvc;
using KuaforYonetim.Data;
using KuaforYonetim.Models;
using Microsoft.EntityFrameworkCore;

namespace KuaforYonetim.Controllers
{
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hizmetler listesi
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler.Include(h => h.Salon).ToListAsync();
            return View(hizmetler);
        }

        // Hizmet ekle
        public IActionResult Create()
        {
            ViewData["Salonlar"] = _context.Salonlar.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hizmet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Salonlar"] = _context.Salonlar.ToList();
            return View(hizmet);
        }

        // Hizmet düzenle
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }
            ViewData["Salonlar"] = _context.Salonlar.ToList();
            return View(hizmet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hizmet hizmet)
        {
            if (id != hizmet.HizmetID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Hizmetler.Any(e => e.HizmetID == hizmet.HizmetID))
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
            ViewData["Salonlar"] = _context.Salonlar.ToList();
            return View(hizmet);
        }

        // Hizmet sil
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _context.Hizmetler.Include(h => h.Salon).FirstOrDefaultAsync(h => h.HizmetID == id);
            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet != null)
            {
                _context.Hizmetler.Remove(hizmet);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
