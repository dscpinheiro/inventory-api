using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Interfaces;
using Inventory.Models;
using Inventory.Web.Requests;
using Inventory.Web.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    [Route("purchases"), ApiController, Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly IShopService _shopService;

        public PurchasesController(IShopService service) => _shopService = service;

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Purchase>>> Get()
        {
            var purchases = await _shopService.GetPurchases();
            return purchases.ToList();
        }

        /// <summary>Submits a purchase of an item.</summary>
        /// <param name="request">The item (and quantity) to be purchased.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PurchaseResponse>> Post(PurchaseRequest request)
        {
            var item = await _shopService.GetItem(request.ItemId.Value);
            if (item == null)
            {
                return NotFound();
            }

            var result = await _shopService.BuyItem(item, request.Quantity.Value, GetBuyerId());

            return new PurchaseResponse
            {
                Status = result.status,
                TotalPrice = result.totalPrice
            };
        }

        private string GetBuyerId()
        {
            var sub = User.Claims.FirstOrDefault(c => c.Type == "sub");
            if (sub != null)
            {
                return sub.Value;
            }

            var clientId = User.Claims.FirstOrDefault(c => c.Type == "client_id");
            if (clientId != null)
            {
                return clientId.Value;
            }

            throw new InvalidOperationException("Could not find subject or client id in token");
        }
    }
}