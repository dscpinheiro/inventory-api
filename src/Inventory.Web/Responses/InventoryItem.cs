using System;

namespace Inventory.Web.Responses
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int AvailableUnits { get; set; }
    }
}