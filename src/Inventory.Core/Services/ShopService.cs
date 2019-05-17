using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Data;
using Inventory.Core.Interfaces;
using Inventory.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Core.Services
{
    public class ShopService : IShopService
    {
        private readonly ApiDbContext _context;
        public ShopService(ApiDbContext context) => _context = context;

        public async Task<Item> GetItem(Guid id) =>
            await _context.Items.SingleOrDefaultAsync(i => i.Id == id);

        public async Task<IEnumerable<Item>> GetItems(int limit, int offset, string name, string description)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => EF.Functions.Like(p.Name, $"%{name}%"));
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(p => EF.Functions.Like(p.Description, $"%{description}%"));
            }

            query = query.OrderBy(p => p.Name).Skip(offset).Take(limit);

            return await query.ToListAsync();
        }

        public async Task<(PurchaseStatus status, int totalPrice)> BuyItem(Item item, int quantity)
        {
            if (item.AvailableUnits == 0)
            {
                return (PurchaseStatus.OutOfStock, 0);
            }

            if (item.AvailableUnits < quantity)
            {
                return (PurchaseStatus.NotEnoughItems, 0);
            }

            item.AvailableUnits -= quantity;
            _context.Items.Update(item);

            var purchase = new Purchase
            {
                Item = item,
                Quantity = quantity,
                TotalPrice = item.Price * quantity
            };

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            return (PurchaseStatus.Completed, purchase.TotalPrice);
        }
    }
}