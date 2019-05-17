using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory.Models;

namespace Inventory.Core.Interfaces
{
    public interface IShopService
    {
        Task<IEnumerable<Item>> GetItems(int limit, int offset, string name, string description);
    }
}