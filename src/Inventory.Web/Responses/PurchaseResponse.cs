using System;
using Inventory.Models;

namespace Inventory.Web.Responses
{
    public class PurchaseResponse
    {
        public PurchaseStatus Status { get; set; }
        public int TotalPrice { get; set; }
    }
}