using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Interfaces;
using Inventory.Models;
using Inventory.Web.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    [Route("inventory"), ApiController, AllowAnonymous]
    public class InventoryController : ControllerBase
    {
        private readonly IShopService _shopService;

        public InventoryController(IShopService service) => _shopService = service;

        /// <summary>Retrieves all items in the inventory, ordered by name.</summary>
        /// <param name="limit">Number of items to be returned.</param>
        /// <param name="offset">Number of items to skip. Can be used to paginate results.</param>
        /// <param name="name">Optional filter. If included, only items whose name contain it will be returned.</param>
        /// <param name="description">Optional filter. If included, only items whose description contain it will be returned.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> Get(int limit = 100, int offset = 0, string name = null, string description = null)
        {
            if (limit <= 0 || limit > 1000)
            {
                return BadRequest($"{nameof(limit)} must be between 1 and 1000");
            }

            if (offset < 0)
            {
                return BadRequest($"{nameof(offset)} must be at least zero");
            }

            var items = await _shopService.GetItems(limit, offset, name, description);
            return items.Select(CreateReadModel).ToList();;
        }

        /// <summary>Retrieves an existing item in the inventory.</summary>
        /// <param name="id">Id of the item to be retrieved.</param>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItem>> Get(Guid id)
        {
            var item = await _shopService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }

            return CreateReadModel(item);
        }

        /// <summary>
        /// Maps the item entity to a model returned to clients.
        /// </summary>
        private InventoryItem CreateReadModel(Item item) => new InventoryItem
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            AvailableUnits = item.AvailableUnits
        };
    }
}