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
        }
    }
}