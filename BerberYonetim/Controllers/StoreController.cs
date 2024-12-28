using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StoreController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["IsAdmin"] = user != null && await _userManager.IsInRoleAsync(user, "Admin");
            return View();
        }

        public IActionResult Barbers()
        {
            var barbers = _context.Stores
                .Where(s => s.Type == StoreType.Barber)
                .Include(s => s.Employees) // Çalışanları dahil et
                .ToList();

            // İşe hazır (boşta olan) çalışanları ViewBag ile gönder
            ViewBag.AvailableEmployees = _context.Employees
                .Where(e => e.StoreId == null) // Çalışanı boşta olanları filtrele
                .ToList();

            return View(barbers);
        }

        [HttpPost]
        public IActionResult AddEmployeeToStore(int storeId, int employeeId)
        {
            var store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (store == null || employee == null)
            {
                return NotFound("Mağaza veya çalışan bulunamadı.");
            }

            var storeType = store.Type;
            employee.StoreId = store.Id; // Çalışanı mağazaya ata
            _context.SaveChanges();
            switch (storeType)
            {
                case StoreType.Barber:
                    return RedirectToAction("Barbers");
                case StoreType.HairDresser:
                    return RedirectToAction("HairDressers");
                default:
                    return RedirectToAction("Index", "Home"); // Varsayılan yönlendirme
            }
        }
        public IActionResult HairDressers()
        {
            var hairDressers = _context.Stores
                .Where(s => s.Type == StoreType.HairDresser)
                .Include(s => s.Employees) // Çalışanları dahil et
                .ToList();

            // İşe hazır (boşta olan) çalışanları ViewBag ile gönder
            ViewBag.AvailableEmployees = _context.Employees
                .Where(e => e.StoreId == null) // Çalışanı boşta olanları filtrele
                .ToList();

            return View(hairDressers);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateStore()
        {
            var availableEmployees = _context.Employees
                .Where(e => e.StoreId == null) // Mağazaya atanmamış çalışanlar
                .ToList();

            var viewModel = new CreateStoreViewModel
            {
                AvailableEmployees = availableEmployees
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStore(CreateStoreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var store = new Store
                {
                    Name = viewModel.Name,
                    Type = viewModel.Type
                };

                _context.Stores.Add(store);
                _context.SaveChanges();

                // Çalışanları mağazaya bağla
                if (viewModel.SelectedEmployeeIds != null && viewModel.SelectedEmployeeIds.Any())
                {
                    var employees = _context.Employees
                        .Where(e => viewModel.SelectedEmployeeIds.Contains(e.Id))
                        .ToList();

                    foreach (var employee in employees)
                    {
                        employee.StoreId = store.Id;
                    }

                    _context.SaveChanges();
                }

                switch (store.Type)
                {
                    case StoreType.Barber:
                        return RedirectToAction("Barbers");
                    case StoreType.HairDresser:
                        return RedirectToAction("HairDressers");
                }
            }
            
            viewModel.AvailableEmployees = _context.Employees
                .Where(e => e.StoreId == null)
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult RemoveEmployee(int employeeId)
        {
            var employee = _context.Employees
                .Include(e => e.Store) // Çalışanın mağazasıyla birlikte alınması
                .FirstOrDefault(e => e.Id == employeeId);
        
            if (employee == null)
            {
                return NotFound("Çalışan bulunamadı.");
            }
            var storeType = employee.Store.Type;
            // Çalışanı mağazadan çıkar (StoreId'yi null yap)
            employee.StoreId = null;
            _context.SaveChanges();

            
            // Mağaza türüne göre yönlendirme
            if (storeType == StoreType.Barber)
            {
                return RedirectToAction("Barbers");
            }
            if (storeType == StoreType.HairDresser)
            {
                return RedirectToAction("HairDressers");
            }

            // Eğer çalışan bir mağazaya bağlı değilse veya başka bir durum varsa
            return RedirectToAction("Index", "Home");
        }


    }
}