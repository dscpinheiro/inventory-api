using System;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Core.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var purchaseEntity = modelBuilder.Entity<Purchase>();
            purchaseEntity.Property(p => p.Id).ValueGeneratedOnAdd();
            purchaseEntity.Property(p => p.ItemId).IsRequired();
            purchaseEntity.Property(p => p.Quantity).IsRequired();
            purchaseEntity.Property(p => p.TotalPrice).IsRequired();
            purchaseEntity.Property(p => p.BuyerId).IsRequired();

            var itemEntity = modelBuilder.Entity<Item>();
            itemEntity.Property(p => p.Name).IsRequired().HasMaxLength(128);
            itemEntity.Property(p => p.Description).HasMaxLength(512);
            itemEntity.Property(p => p.Price).IsRequired();

            itemEntity.HasData(new []
            {
                new Item
                {
                    Id = Guid.Parse("64876c5f-0fc1-4a48-974e-da66d9c05630"),
                    Name = "+5 Dexterity Vest",
                    Description = "+5 Dexterity Vest",
                    Price = 10,
                    AvailableUnits = 25
                },
                new Item
                {
                    Id = Guid.Parse("21f9427a-34b3-448f-afc4-792be2014b6e"),
                    Name = "Elixir of the Mongoose",
                    Description = "Elixir of the Mongoose",
                    Price = 15,
                    AvailableUnits = 20
                },
                new Item
                {
                    Id = Guid.Parse("09af8d3f-85bc-46a7-8d48-79ba78a01de6"),
                    Name = "Conjured Mana Cake",
                    Description = "Conjured Mana Cake",
                    Price = 25,
                    AvailableUnits = 10
                },
                new Item
                {
                    Id = Guid.Parse("84a46910-bbe7-4cba-a4f1-bbe1228036c7"),
                    Name = "Aged Brie",
                    Description = "Aged Brie",
                    Price = 5,
                    AvailableUnits = 50
                },
                new Item
                {
                    Id = Guid.Parse("4bf93e23-44b8-402e-86da-f690abb1f0d5"),
                    Name = "Backstage passes to a TAFKAL80ETC concert",
                    Description = "Backstage passes to a TAFKAL80ETC concert",
                    Price = 100,
                    AvailableUnits = 10
                }
            });
        }
    }
}