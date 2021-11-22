﻿using Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.MsSql
{
    public class DataContext:DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DataContext() { }
        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FoodDeliveryDB;Trusted_Connection=True;");
        }

    }
}