using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class CreateStoreViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public StoreType Type { get; set; }

    public List<int> SelectedEmployeeIds { get; set; } = new List<int>(); // Seçilen çalışanların ID'leri

    public List<Employee> AvailableEmployees { get; set; } = new List<Employee>(); // Mevcut çalışanlar
}