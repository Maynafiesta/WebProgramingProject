using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AppointmentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Randevu alma sayfası (GET)
    public IActionResult Create()
    {
        var stores = _context.Stores.Include(s => s.Employees).ToList();
        ViewBag.Stores = stores;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int storeId, int employeeId, DateTime appointmentTime)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        // DateTime'ı UTC'ye çevir
        appointmentTime = DateTime.SpecifyKind(appointmentTime, DateTimeKind.Utc);

        // Çalışanın belirtilen zaman aralığında başka randevusu var mı?
        var overlappingAppointments = _context.Appointments
            .Where(a => a.EmployeeId == employeeId &&
                        ((appointmentTime >= a.AppointmentTime && appointmentTime < a.AppointmentEndTime) ||
                         (appointmentTime.AddHours(1) > a.AppointmentTime && appointmentTime.AddHours(1) <= a.AppointmentEndTime)))
            .ToList();

        if (overlappingAppointments.Any())
        {
            TempData["ErrorMessage"] = "Bu çalışan, seçilen zaman aralığında başka bir randevuya sahiptir.";
            return RedirectToAction("Create");
        }

        // Randevuyu oluştur
        var appointment = new Appointment
        {
            StoreId = storeId,
            EmployeeId = employeeId,
            UserId = user.Id,
            AppointmentTime = appointmentTime
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu.";
        return RedirectToAction("Create");
    }


    
    public async Task<IActionResult> MyAppointments()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var appointments = _context.Appointments
            .Where(a => a.UserId == user.Id)
            .Include(a => a.Store)
            .Include(a => a.Employee)
            .ToList();

        return View(appointments);
    }
}