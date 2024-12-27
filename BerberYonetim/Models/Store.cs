namespace WebApplication1.Models;

public class Store
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public StoreType Type { get; set; }
}