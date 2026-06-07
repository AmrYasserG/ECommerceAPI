using ECommerceAPI.Common.Models;
using ECommerceAPI.DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.DAL.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Precision
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            // CartItem → Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → OrderItems
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → Product (Restrict to avoid multiple cascade paths)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
                new Category { Id = 2, Name = "Clothing",    Description = "Apparel and accessories" },
                new Category { Id = 3, Name = "Books",       Description = "Physical and digital books" },
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop",              Description = "15-inch laptop with 16GB RAM",              Price = 999.99m, Stock = 25,  CategoryId = 1 },
                new Product { Id = 2, Name = "Wireless Headphones", Description = "Noise-cancelling over-ear headphones",      Price = 149.99m, Stock = 50,  CategoryId = 1 },
                new Product { Id = 3, Name = "Smartphone",          Description = "Latest model with 128GB storage",           Price = 799.99m, Stock = 40,  CategoryId = 1 },
                new Product { Id = 4, Name = "T-Shirt",             Description = "100% cotton crew neck t-shirt",             Price = 19.99m,  Stock = 200, CategoryId = 2 },
                new Product { Id = 5, Name = "Jeans",               Description = "Slim fit denim jeans",                      Price = 49.99m,  Stock = 100, CategoryId = 2 },
                new Product { Id = 6, Name = "C# in Depth",         Description = "Comprehensive guide to C# programming",     Price = 39.99m,  Stock = 75,  CategoryId = 3 },
                new Product { Id = 7, Name = "Clean Code",          Description = "A handbook of agile software craftsmanship", Price = 34.99m,  Stock = 60,  CategoryId = 3 },
            };

            modelBuilder.Entity<Category>().HasData(categories);
            modelBuilder.Entity<Product>().HasData(products);
        }
    }
}
