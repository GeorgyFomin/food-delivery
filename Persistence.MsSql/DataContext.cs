using Entities.Domain;
using Microsoft.EntityFrameworkCore;


namespace Persistence.MsSql
{
    public class DataContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DataContext() { }
        public DataContext(DbContextOptions options) : base(options) { }
    }
}