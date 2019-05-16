using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Interfaces;
using Inventory.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    [Route("inventory"), ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IShopService _shopService;

        public InventoryController(IShopService service) => _shopService = service;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Item>>> Get(int limit = 100, int offset = 0, string name = null, string description = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                return BadRequest($"{nameof(limit)} must be between 1 and 1000");
            }

            if (offset < 0)
            {
                return BadRequest($"{nameof(offset)} must be at least zero");
            }

            var items = await _shopService.GetAll(limit, offset, name, description);
            return items.ToList();
        }
    }
}