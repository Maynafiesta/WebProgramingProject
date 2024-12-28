using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Çalışan Ekleme İşlemi (POST)
    public IActionResult Create()
    {
        var viewModel = new CreateEmployeeViewModel
        {
            AvailableSkills = _context.Skills.ToList() // Tüm yetenekleri listele
        };

        return View(viewModel);
    }
    // Çalışan Ekleme İşlemi (POST)
    [HttpPost]
    public IActionResult Create(CreateEmployeeViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var employee = new Employee
            {
                Name = viewModel.Name
            };

            // Seçilen yetenekleri çalışana ata
            if (viewModel.SelectedSkillIds != null && viewModel.SelectedSkillIds.Any())
            {
                var skills = _context.Skills
                    .Where(s => viewModel.SelectedSkillIds.Contains(s.Id))
                    .ToList();

                employee.Skills = skills;
            }

            _context.Employees.Add(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Hata durumunda yetenekleri tekrar sağla
        viewModel.AvailableSkills = _context.Skills.ToList();
        return View(viewModel);
    }

    // Çalışanları Listeleme (GET)
    public IActionResult Index()
    {
        var employees = _context.Employees
            .Include(e => e.Skills) // Yeteneklerle birlikte al
            .ToList();

        return View(employees);
    }
}