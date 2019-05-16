using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Data;
using Inventory.Core.Interfaces;
using Inventory.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Core.Services
{
    public class ShopService : IShopService
    {
        private readonly ApiDbContext _context;
        public ShopService(ApiDbContext context) => _context = context;

        public async Task<IEnumerable<Item>> GetAll(int limit, int offset, string name, string description)
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
    }
}