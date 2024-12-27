using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly StoreManagementSystemDbContext _context;

    public HomeController(ILogger<HomeController> logger, StoreManagementSystemDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Barbers()
    {
        var barbers = _context.Stores.Where(s => s.Type == StoreType.Barber).ToList();
        return View(barbers);
    }
    public IActionResult HairDressers()
    {
        var hairDressers = _context.Stores.Where(s => s.Type == StoreType.HairDresser).ToList();
        return View(hairDressers);
    }
    public IActionResult CreateStore()
    {
        return View("CreateStore");
    }

    public IActionResult CreateNewStore(Store store)
    {
        if (!ModelState.IsValid)
        {
            throw new Exception("ModelState not valid");
        }

        store.Id = Guid.NewGuid();
        _context.Stores.Add(store);
        _context.SaveChanges();
        if (store.Type == StoreType.HairDresser)
        {
            return RedirectToAction("HairDressers");
        }  
            
        if (store.Type == StoreType.Barber)
        {
            return RedirectToAction("Barbers");
        }

        return View(store);
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