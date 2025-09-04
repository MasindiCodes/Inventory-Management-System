using System;
using System.Collections.Generic;

namespace InventoryManagementSystem
{
    public class NonPerishableProduct : Product
    {
        public override string GetInfo() =>
            $"{Name} ({Category}) - R{Price:F2} x {Quantity}";
    }
}

