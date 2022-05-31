using Entities.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Persistence.MsSql
{
    public class DataContext :
        DbContext
        //IdentityDbContext<ApplicationUser>
    {
        //Известно, что ссылка на описанные ниже DbSet никогда не будет null. Поэтому можно эти свойства описать как { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;// => Set<Menu>();
        public DbSet<MenuItem> MenuItems { get; set; } = null!;// => Set<MenuItem>();
        public DbSet<Order> Orders { get; set; } = null!;// => Set<Order>();
        public DbSet<OrderItem> OrderItems { get; set; } = null!;// => Set<OrderItem>();
        public DbSet<Discount> Discounts { get; set; } = null!;// => Set<Discount>();
        public DbSet<Employee> Employees { get; set; } = null!;// => Set<Employee>();
        public DbSet<Product> Products { get; set; } = null!;// => Set<Product>();
        public DbSet<Ingredient> Ingredients { get; set; } = null!;// => Set<Ingredient>();
        public DbSet<Delivery> Deliveries { get; set; } = null!;//=> Set<Delivery>();
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIngredient>().HasKey(k => new { k.IngredientId, k.ProductId });
            modelBuilder.Entity<Order>().OwnsOne(p => p.PhoneNumder);
        }
    }
}
