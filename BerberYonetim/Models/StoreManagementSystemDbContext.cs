using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public class StoreManagementSystemDbContext : DbContext
{
    public DbSet<Store> Stores { get; set; }

    public StoreManagementSystemDbContext(DbContextOptions<StoreManagementSystemDbContext> options) : base(options)
    {
    }
}