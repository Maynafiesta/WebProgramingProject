namespace WebApplication1.Models;

public class BarbersViewModel
{
    public List<Store> Stores { get; set; } = new List<Store>();
    public List<Employee> AvailableEmployees { get; set; } = new List<Employee>();
}