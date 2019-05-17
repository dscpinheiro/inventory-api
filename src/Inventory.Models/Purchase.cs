using System;

namespace Inventory.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
        public string BuyerId { get; set; }
    }
}
