using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Web.Controllers
{
    [Route("purchases"), ApiController, Authorize]
    public class PurchasesController : ControllerBase
    {
        /// <summary>Submits a purchase of an item.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Post() => Ok();
    }
}