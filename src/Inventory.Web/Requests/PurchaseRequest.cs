using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Inventory.Web.Requests
{
    public class PurchaseRequest
    {
        [JsonProperty("itemId"), Required]
        public Guid? ItemId { get; set; }

        [JsonProperty("quantity"), Required, Range(1, 50)]
        public int? Quantity { get; set; }
    }
}