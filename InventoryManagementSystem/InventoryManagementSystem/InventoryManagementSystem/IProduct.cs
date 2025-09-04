using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    // interface that is a blueprint for products that ensures all the products in the system hae these core properties
    public interface IProduct
    {
        int Id { get; } // read only incapsulation or partial 
        string Name { get; }
        string Category { get; }
        int Stock { get; }
        decimal Price { get; }
    }
}
