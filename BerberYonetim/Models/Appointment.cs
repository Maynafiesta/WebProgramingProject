namespace WebApplication1.Models;

public class Appointment
{
    public int Id { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } // Mağaza ile ilişki

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } // Çalışan ile ilişki

    public string UserId { get; set; }
    public DateTime AppointmentTime { get; set; } // Başlangıç zamanı
    public DateTime AppointmentEndTime => AppointmentTime.AddHours(1); // Bitiş zamanı (1 saat sonra)
}