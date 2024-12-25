using Microsoft.EntityFrameworkCore;
using KuaforYonetim.Models;

namespace KuaforYonetim.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Salon> Salonlar { get; set; }
    }
}