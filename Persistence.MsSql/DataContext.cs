using Entities.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Persistence.MsSql
{
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole, string> //DbContext
    {
        //Известно, что ссылка на описанные ниже DbSet никогда не будет null. Поэтому можно эти свойства описать как { get; set; } = null!;
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Discount> Discounts => Set<Discount>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIngredient>().HasKey(k => new { k.IngredientId, k.ProductId });
            modelBuilder.Entity<Order>().OwnsOne(p => p.PhoneNumder);
            modelBuilder.Entity<IdentityUserLogin<string>>(o => o.HasNoKey());
            modelBuilder.Entity<IdentityUserRole<string>>(o => o.HasNoKey());
            modelBuilder.Entity<IdentityUserToken<string>>(o => o.HasNoKey());
        }
    }
}
