using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    public enum Category
    {
        Hygiene = 1,
        Dairy,
        Beverages,
        Electronics,
        Stationery,
        Clothing
    }

    public abstract class Product : IComparable<Product>
    {
        // declaring properties, get lets you read the value, set lets you assign a value
        private static int nextId = 1; // auto incrementing ID
        // property declaration that stores an integer but unly code inside the class can change the value of the Id
        public int Id { get; private set; }
        public string Name { get; set; } // encapsulation
        public Category Category { get; set; } // change from string to enum
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; } = 0; // default is 0 so that when you add it does not give weird results
        public int ReorderLevel { get; set; }
        // method that increments by 1 everytime a new product is added
        public Product()
        {
            Id = nextId++;
        }

        // replaces the default behaviour of the Object.ToString() method
        public override string ToString()
        {
            // formats the properties into a readable string
            return $"{Name} ({Category}) - R{Price}";
        }

        // shows detailed information about the product
        // Override the base GetInfo() method to include the expiry date for perishable items.
        public abstract string GetInfo();

        //implements a built in interface that lets you compare two products by their prices
        public int CompareTo(Product other)
        {
            return Price.CompareTo(other.Price);
        }
        public void UpdateStock(int quantityToAdd)
        {
            // updates the stock by adding the new quantity to whatever number of stock was there
            Stock += quantityToAdd;

            // Optional: prevent negative stock
            if (Stock < 0)
                Stock = 0;
        }


    }
}