﻿using System;
using System.Collections.Generic;

namespace Inventory.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int AvailableUnits { get; set; }
    }
}
