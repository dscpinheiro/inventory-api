using System;
using Inventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Core.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                    Quantity = 25
                },
                new Item
                {
                    Id = Guid.Parse("21f9427a-34b3-448f-afc4-792be2014b6e"),
                    Name = "Elixir of the Mongoose",
                    Description = "Elixir of the Mongoose",
                    Price = 15,
                    Quantity = 20
                },
                new Item
                {
                    Id = Guid.Parse("f0de704d-5d5e-4364-bd01-466f0022e8ff"),
                    Name = "Sulfuras, Hand of Ragnaros",
                    Description = "Sulfuras, Hand of Ragnaros",
                    Price = 20,
                    Quantity = 15
                },
                new Item
                {
                    Id = Guid.Parse("09af8d3f-85bc-46a7-8d48-79ba78a01de6"),
                    Name = "Conjured Mana Cake",
                    Description = "Conjured Mana Cake",
                    Price = 25,
                    Quantity = 10
                }
            });
        }
    }
}