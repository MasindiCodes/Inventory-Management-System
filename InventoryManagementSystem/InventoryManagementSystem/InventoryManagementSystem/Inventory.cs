using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem
{
    public class Order
    {
        private static int nextOrderId = 1000;
        public int OrderId { get; private set; } // encapsulation
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public Order()
        {
            OrderId = nextOrderId++;
        }
    }

    // Event arguments for low stock
    public class LowStockEventArgs : EventArgs // inheritance
    {
        public Product Product { get; set; }
        public LowStockEventArgs(Product product)
        {
            // assigning property
            Product = product;
        }
    }

    // Event arguments for processed order
    public class OrderProcessedEventArgs : EventArgs // inheritance
    {
        public Order Order { get; set; }
        public OrderProcessedEventArgs(Order order)
        {
            // assigning property
            Order = order;
        }
    }

    public class Inventory
    {
        // stores all the products added by the user.
        private List<Product> products = new List<Product>();
        // stores all the orders processed
        private List<Order> orders = new List<Order>();

        // Events
        public event EventHandler<LowStockEventArgs> LowStockEvent;
        public event EventHandler<OrderProcessedEventArgs> OrderProcessedEvent;

        // Adds a product
        public void AddProduct(Product product)
        {
            products.Add(product);
            CheckStock(product); // optionally trigger stock check immediately
        }

        // Checks stock for a product
        public void CheckStock(Product product)
        {
            if (product.Stock <= product.ReorderLevel)
            {
                Console.WriteLine($"ALERT: Low stock for {product.Name} (Stock: {product.Stock})");
                LowStockEvent?.Invoke(this, new LowStockEventArgs(product));
            }
        }

        // Processes an order
        public bool ProcessOrder(Order order)
        {
            Product product = products.FirstOrDefault(p => p.Id == order.ProductId);

            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return false;
            }

            if (product.Stock >= order.Quantity)
            {
                product.Stock -= order.Quantity;
                orders.Add(order);

                Console.WriteLine($"Order processed for {order.Quantity} x {product.Name}");
                OrderProcessedEvent?.Invoke(this, new OrderProcessedEventArgs(order));

                CheckStock(product);
                return true;
            }
            else
            {
                Console.WriteLine("Not enough stock to process order.");
                return false;
            }
        }

        // Getting all the products
        public List<Product> GetAllProducts()
        {
            return products;
        }

        // Returns products grouped by category
        public IEnumerable<IGrouping<Category, Product>> GetProductsByCategory()
        {
            return products.GroupBy(p => p.Category);
        }

        // Returns products sorted by price
        public List<Product> GetProductsSortedByPrice()
        {
            return products.OrderBy(p => p.Price).ToList();
        }

        // Getting the total inventory value
        public decimal GetTotalInventoryValue()
        {
            return products.Sum(p => p.Price * p.Quantity);
        }

        // Displaying all the orders orders
        public void ListOrders()
        {
            if (!orders.Any())
            {
                Console.WriteLine("No orders available.");
                return;
            }

            Console.WriteLine("\n=== Orders List ===");
            foreach (var order in orders)
            {
                Console.WriteLine($"OrderID: {order.OrderId}, ProductID: {order.ProductId}, Quantity: {order.Quantity}");
            }
        }
    }
}
