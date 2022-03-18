using Entities.Domain;
using Microsoft.EntityFrameworkCore;


namespace Persistence.MsSql
{
    public class DataContext : DbContext
    {
        //public DbSet<Menu> Menus => Set<Menu>();//{ get; set; } = null!;
        //public DbSet<MenuItem> MenuItems => Set<MenuItem>();// { get; set; } = null!;
        //public DbSet<Order> Orders => Set<Order>();//{ get; set; } = null!;
        //public DbSet<OrderItem> OrderItems => Set<OrderItem>();//{ get; set; } = null!;
        public DbSet<Discount> Discounts => Set<Discount>();//{ get; set; } = null!;
        public DbSet<Employee> Employees => Set<Employee>();//{ get; set; } = null!;
        public DbSet<Product> Products => Set<Product>();// {get; set; } = null!; 
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();// { get; set; } = null!;
        public DbSet<Delivery> Deliveries => Set<Delivery>();//{ get; set; } = null!;
        public DataContext() { }
        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIngredient>().HasKey(k => new { k.IngredientId, k.ProductId });
        }
    }
}
