using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace InventoryManagementSystem
{
    internal class Program
    {

        public class User
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }

            public User(string userName, string password, string role)
            {
                UserName = userName;
                Password = password;
                Role = role;
            }

            public bool Authenticate(string inputPassword)
            {
                return Password == inputPassword;
            }
        }

        // Inventory object to manage products and orders
        static Inventory inventory = new Inventory();
        // flag to control the menu loop
        static bool running = true;
        static User currentUser = null;

        static List<User> users = new List<User>
        {
            new User("admin", "admin123", "Administrator"),
            new User("manager", "manager123", "Manager"),
            new User("staff", "staff123", "Staff")
        };


        // ------------------ LOGIN METHODS -------------------
        public static void Login()
        {
            Console.WriteLine("===== Login =====");
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.UserName == username);

            if (user != null && user.Authenticate(password))
            {
                currentUser = user;
                Console.WriteLine($"Welcome {currentUser.UserName} ({currentUser.Role})"); ;
            }
            else
            {
                Console.WriteLine("Invalid username or password. Please try again.");
                Login(); // retry until success
            }
        }


        // Method to show the main menu
        public static void ShowMenu()
        {
            while (running)
            {
                Console.WriteLine("======================================================");
                Console.WriteLine($"Welcome {currentUser.UserName} ({currentUser.Role})");
                Console.WriteLine("======================================================");
                Console.WriteLine("1. Add Product (Admin only)");
                Console.WriteLine("2. Display Products");
                Console.WriteLine("3. Display Products by Category");
                Console.WriteLine("4. Display Products by Price");
                Console.WriteLine("5. Calculate Total Value");
                Console.WriteLine("6. Process an Order (Manager/Admin only)");
                Console.WriteLine("7. Logout");
                Console.WriteLine("======================================================");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            if (currentUser.Role == "Administrator")
                                AddProduct();
                            else
                                Console.WriteLine("Access Denied: Only Admin can add products.");
                            break;
                        case 2:
                            DisplayProducts();
                            break;
                        case 3:
                            DisplayProductsByCategory();
                            break;
                        case 4:
                            DisplayProductsByPrice();
                            break;
                        case 5:
                            CalculateTotalValueOfProducts();
                            break;
                        case 6:
                            if (currentUser.Role == "Manager" || currentUser.Role == "Administrator")
                                ProcessOrder();
                            else
                                Console.WriteLine("Access Denied: Only Manager/Admin can process orders.");
                            break;
                        case 7:
                            currentUser = null;
                            Console.WriteLine("Logged out successfully.");
                            Thread.Sleep(1000);
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid number.");
                }
                Console.WriteLine();
            }
        }

        // Method to add a product to the inventory
        public static void AddProduct()
        {
            // Displays category options to the user
            Console.WriteLine("Select a category:");
            // foreach loop goes through each of the values from the Enum.GetValues(typeof(Category) which gets all the possible values of the category enum 
            //basically it displays all enum categories with numbers, so the user can type a number to pick one.
            foreach (int i in Enum.GetValues(typeof(Category)))
            {
                Console.WriteLine($"{i}. {Enum.GetName(typeof(Category), i)}");
            }

            Category category;
            while (true)
            {
                // Converting user input to integer and checking if it is a valid category
                if (int.TryParse(Console.ReadLine(), out int catChoice)
                    // checks if the integer matches one of the defined enum values
                    && Enum.IsDefined(typeof(Category), catChoice))
                {
                    category = (Category)catChoice;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select a valid category number.");
                }
            }

            Console.WriteLine("Enter the name of the product:");
            string name = Console.ReadLine();
            // input validation to ensure that the product name is not left empty
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Product name cannot be empty.");
                return;
            }

            // Exception Handling
            decimal price = 0;
            // loop keeps running as long as the validPrice is false
            // it will keep going until the user enters a vaild number
            bool validPrice = false;
            while (!validPrice)
            {
                // code that might throw an error if the user types something that is not a number
                try
                {
                    Console.Write("Enter price: ");
                    price = decimal.Parse(Console.ReadLine());
                    validPrice = true;
                // if the conversion works, it sets the validPrice to true which causes it to exit the loop
                }
                // what happens if the error occurs
                catch (FormatException)
                {
                    Console.WriteLine("Invalid price. Please enter a number.");
                }
            }

            // Exception Handling
            int quantity = 0;
            bool validQuantity = false;
            while (!validQuantity)
            // loop keeps running as long as the validQuantity is false
            {
            // code that might throw an error if the user types something that is not a number
                try
                {
                    Console.Write("Enter quantity: ");
                    quantity = int.Parse(Console.ReadLine());
                    validQuantity = true;
             // if the conversion works, it sets the validPrice to true which causes it to exit the loop
                }
             // what happens if the error occurs
                catch (FormatException)
                {
                    Console.WriteLine("Invalid quantity. Please enter a number.");
                }
            }

            Console.Write("Enter current stock: ");
            int stock = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter reorder level: ");
            int reorderLevel = Convert.ToInt32(Console.ReadLine());

            Console.Write("Is this product perishable? (y/n): ");
            string perishableInput = Console.ReadLine().ToLower();

            Product newProduct;
            if (perishableInput == "y")
            {
                var perishable = new PerishableProduct();
                Console.Write("Enter expiry date (yyyy-mm-dd): ");
                DateTime.TryParse(Console.ReadLine(), out DateTime expiry);
                perishable.ExpiryDate = expiry;
                newProduct = perishable;
            }
            else
            {
                newProduct = new NonPerishableProduct();
            }
            // Assigning properties
            newProduct.Name = name;
            newProduct.Category = category;
            newProduct.Price = price;
            newProduct.Quantity = quantity;
            newProduct.Stock = stock;
            newProduct.ReorderLevel = reorderLevel;
            newProduct.UpdateStock(quantity);

            // Add product to inventory
            inventory.AddProduct(newProduct);

            Console.WriteLine("====================================================================");
            Console.WriteLine($"{name} has been added to category {category}.");
            Console.WriteLine($"Product added! ID: {newProduct.Id}, Total stock: {newProduct.Stock}");
            Console.WriteLine("====================================================================");
        }

        public static void ProcessOrder()
        {
            Console.Write("Enter Product ID: ");
            int productId = int.Parse(Console.ReadLine());

            Console.Write("Enter Quantity: ");
            int quantity = int.Parse(Console.ReadLine());

            Order order = new Order
            {
                // assigning properties
                ProductId = productId,
                Quantity = quantity
            };

            inventory.ProcessOrder(order);
        }

        public static void DisplayProducts()
        {
            // calling a method on the inventory object and storing the result in a variable
            var products = inventory.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("No products available to display.");
                return;
            }

            Console.WriteLine("\n===== Product List =====");
            foreach (var product in products)
            {
                Console.WriteLine("================================");
                // calling the getInfo method on the product object and prints the result to the console
                Console.WriteLine(product.GetInfo());
                Console.WriteLine("================================");
            }
        }

        public static void DisplayProductsByCategory()
        {
            /* groups all products in the inventory by their category property it then returns a colllection of groups
            each group has a key and the collection of products in that category
           var grouped stores this grouped data in a variable so the user can loop throught it latert */
            var grouped = inventory.GetProductsByCategory();

            if (!grouped.Any() || grouped.All(g => !g.Any()))
            {
                Console.WriteLine("No products in inventory.");
                return;
            }

            Console.WriteLine("\n===== Products Grouped by Category =====");
            foreach (var group in grouped)
            {
                Console.WriteLine($"Category: {group.Key}");
                foreach (var p in group)
                {
                    Console.WriteLine($"- {p.Name}");
                }
            }
        }

        public static void DisplayProductsByPrice()
        {
            // calls the GetProductsSortedByPrice() from the inventory class and stores the result in a variable named products
            var products = inventory.GetProductsSortedByPrice();

            if (products.Count == 0)
            {
                Console.WriteLine("No products available to display.");
                return;
            }

            Console.WriteLine("\n===== Products Sorted by Price =====");
            foreach (var p in products)
            {
                Console.WriteLine($"Name: {p.Name}");
                Console.WriteLine($"Category: {p.Category}");
                //p.Price:F2 is a format specifier where F stands for fixed point format and the 2 means to 2 decimal places
                Console.WriteLine($"Price: R{p.Price:F2}");
                Console.WriteLine($"Quantity: {p.Quantity}");
                Console.WriteLine("================================");
            }
        }

        public static void CalculateTotalValueOfProducts()
        {
            decimal totalValue = inventory.GetTotalInventoryValue();
            Console.WriteLine($"Total inventory value: R{totalValue:F2}");
        }

        // thread that monitors the stocklevel and if it is low an alert will be display to the user
        public static void MonitorStockLevels()
        {
            while (running)
            {
                var products = inventory.GetAllProducts();
                foreach (var product in products)
                {
                    if (product.Stock <= product.ReorderLevel)
                    {
                        Console.WriteLine($"Alert: {product.Name} stock is low ({product.Stock} remaining).");
                    }
                }
                Thread.Sleep(3000);
            }
        }

        static void Main(string[] args)
        {
            // Start stock monitoring thread
            Thread stockThread = new Thread(MonitorStockLevels);
            stockThread.IsBackground = true;
            stockThread.Start();

            // Subscribe to events
            inventory.LowStockEvent += (s, e) =>
            {
                Console.WriteLine($"[EVENT] Low Stock for {e.Product.Name} (Stock: {e.Product.Stock})");
            };

            inventory.OrderProcessedEvent += (s, e) =>
            {
                Console.WriteLine($"[EVENT] Order {e.Order.OrderId} processed for ProductID {e.Order.ProductId}");
            };

            // 🔹 Require login before showing menu
            Login();
            ShowMenu();

            Console.ReadKey();
        }
    }
}
