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
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.LogTo(Console.WriteLine);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Ingredient>().HasOne(i => i.Product).WithMany(p => p.Ingredients).OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<Ingredient>().HasOne(i=>i.ProductId).With
            //Ingredient ingredient1 = new() { Id = 1, Name = "1" }, ingredient2 = new() { Id = 2, Name = "2" }, ingredient3 = new() { Id = 3, Name = "3" };
            //Product product1 = new()
            //{
            //    Id = 1,
            //    Name = "one",
            //    Price = 1m,
            //    Weight = 1.0
            //},
            //product2 = new()
            //{
            //    Id = 2,
            //    Name = "two",
            //    Price = 2m,
            //    Weight = 2.0
            //},
            //product3 = new()
            //{
            //    Id = 3,
            //    Name = "three",
            //    Price = 3m,
            //    Weight = 3.0
            //},
            //product4 = new()
            //{
            //    Id = 4,
            //    Name = "four",
            //    Price = 4m,
            //    Weight = 4.0
            //};
            //modelBuilder.Entity<Product>().HasData(new[] { product1, product2, product3, product4 });
            //modelBuilder.Entity<Ingredient>().HasData(new[] { ingredient1, ingredient2, ingredient3, new() { Id = 4, Name = "4" } });
            //modelBuilder
            //    .Entity<Product>()
            //    .HasMany(p => p.Ingredients)
            //    .WithMany(i => i.Products)
            //    .UsingEntity(j => j
            //    .HasData(
            //        new object[]
            //        {
            //           new { ProductsId = 4, IngredientsId = 4 },
            //           new { ProductsId = 1, IngredientsId = 3 },
            //           new { ProductsId = 3, IngredientsId = 3 },
            //           new {IngredientsId=1, ProductsId=1},
            //           new {IngredientsId=1, ProductsId=2},
            //           new {IngredientsId=1, ProductsId=3},
            //           new {IngredientsId=1, ProductsId=4}

            //           //new { ProductsID = 3, IngredientsID = 1 }//,
            //           //new { ProductsID = 2, IngredientsID = 4 }//,
            //           //new { ProductsID = 3, IngredientsID = 4 },
            //           //new { ProductsID = 4, IngredientsID = 4 },
            //           //new { ProductsID = 4, IngredientsID = 3 }
            //        }
            //    ));
            //modelBuilder.Entity<ProductIngredient>()
            //.HasKey(t => new { t.ProductId, t.IngredientId });
            //modelBuilder.Entity<ProductIngredient>()
            //    .HasOne(pi => pi.Product)
            //    .WithMany(p => p.ProductIngredients).
            //    HasForeignKey(pi => pi.ProductId);
            //modelBuilder.Entity<ProductIngredient>()
            //    .HasOne(pi => pi.Ingredient)
            //    .WithMany(t => t.ProductIngredients)
            //    .HasForeignKey(pi => pi.IngredientId);
            //modelBuilder.Entity<Product>().HasData(new[] { product1, product2, product3, product4 });
            //modelBuilder.Entity<Ingredient>().HasData(new[] { ingredient1, ingredient2, ingredient3, new() { Id = 4, Name = "4" } });
            //modelBuilder.Entity<ProductIngredient>().HasData(new[] {new { }});// new { IngredientId = 1 }, new {ProductId=2,IngredientId=3 }
            //modelBuilder
            //    .Entity<Product>()
            //    .HasMany(p => p.Ingredients)
            //    .WithMany(p => p.Products)
            //    .UsingEntity<ProductIngredient>(
            //    j => j
            //    .HasOne(pi => pi.Ingredient)
            //    .WithMany(i => i.ProductIngredients)
            //    .HasForeignKey(pi => pi.IngredientId),
            //    j => j
            //    .HasOne(pi => pi.Product)
            //    .WithMany(p => p.ProductIngredients)
            //    .HasForeignKey(pi => pi.ProductId));
        }
    }
}
