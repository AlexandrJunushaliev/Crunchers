using Microsoft.EntityFrameworkCore;

namespace Crunchers.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
         
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
         
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ordersdb;Username=postgres;Password=password");
        }
    }
}