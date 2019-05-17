using System.Linq;
using System.Threading.Tasks;
using Inventory.Core.Interfaces;
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

        /// <summary>Submits a purchase of an item.</summary>
        /// <param name="request">The item (and quantity) to be purchased.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PurchaseResponse>> Post(PurchaseRequest request)
        {
            var item = await _shopService.GetItem(request.ItemId);
            if (item == null)
            {
                return NotFound();
            }

            var userId = User.Claims.First(c => c.Type == "sub").Value;
            var result = await _shopService.BuyItem(item, request.Quantity, userId);

            return Ok(new PurchaseResponse
            {
                Status = result.status,
                TotalPrice = result.totalPrice
            });
        }
    }
}