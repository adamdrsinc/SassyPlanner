using currentworkingsassyplanner.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace currentworkingsassyplanner.Data
{
    public class SPContext : IdentityDbContext
    {
        public SPContext(DbContextOptions<SPContext> options) : base(options)
        {

        }

        // Add tables here e.g. DbSet<Customer> customers { get; set; }
        // -> here
        
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<CustomerOrder> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        [NotMapped]
        public DbSet<OrderConf> OrderConfs { get; set; }
        // End

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BasketItem>().ToTable("BasketItem");
            modelBuilder.Entity<Basket>().ToTable("Basket");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Discount>().ToTable("Discount");
            modelBuilder.Entity<CustomerOrder>().ToTable("CustomerOrder");
            modelBuilder.Entity<Product>().ToTable("Product");

        }
    }
}
    