using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    // Polymorphism in action
    /*
     PerishableProduct inherits from Product, so it’s a specialized type of Product
     Product has a virtual method GetInfo() which gives default info about a product
     By overriding GetInfo() in PerishableProduct, we change how it behaves 
     (adds the expiry date) while still keeping the basic info from Product
     */
    public class PerishableProduct : Product // inheritance
    {
        public DateTime ExpiryDate { get; set; }
        // overrides the abstract GetInfo method in the base product class
        public override string GetInfo() => // abstraction
            // string interpolation that describes a perishable product 
       $"{Name} ({Category}) - R{Price:F2} x {Quantity} | Exp: {ExpiryDate:yyyy-MM-dd}";
    }
    public class NonPerishableProduct : Product
    {
        public override string GetInfo() => // abstraction
      $"{Name} ({Category}) - R{Price:F2} x {Quantity}";
    }

}
