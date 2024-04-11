using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

// Define a class to represent a money item
class MoneyItem
{
    public MoneyItem(string title, double amount, string month, string type)
    {
        Title = title;
        Amount = amount;
        Month = month;
        Type = type;
    }

    public string Title { get; set; }
    public double Amount { get; set; }
    public string Month { get; set; }
    public string Type { get; set; }
}

// Define a class to manage money tracking functionality
class MoneyTracker
{
    private string filePath = @"C:\Users\husni\source\repos\CashFlowTracker\money_data.txt";
    public List<MoneyItem> items;

    // Constructor to initialize the MoneyTracker class
    public MoneyTracker()
    {
        items = new List<MoneyItem>();
    }

    // Method to display items list with total income and total expense
    public void ShowItemList(string itemType = "ALL", string sortBy = "month")
    {
        // Load data from the file
        LoadFromFile();

        // Define column widths for padding
        const int titleWidth = 20;
        const int amountWidth = 15;
        const int monthWidth = 15;
        const int typeWidth = 10;

        // Header
        Console.WriteLine($"{"Title".PadRight(titleWidth)} {"Amount".PadRight(amountWidth)} {"Month".PadRight(monthWidth)} {"Type".PadRight(typeWidth)}");

        // Separator
        Console.WriteLine(new string('-', titleWidth + amountWidth + monthWidth + typeWidth));

        // Initialize variables for total income and total expense
        double totalIncome = 0;
        double totalExpense = 0;

        // Filter items based on item type
        var filteredItems = items;
        if (itemType != "ALL")
        {
            filteredItems = items.Where(item => item.Type == itemType).ToList();
        }

        // Sort items based on sorting criteria
        switch (sortBy)
        {
            case "month":
                filteredItems = filteredItems.OrderBy(item => DateTime.ParseExact(item.Month, "MMMM", CultureInfo.CurrentCulture).Month).ToList();
                break;
            case "amount":
                filteredItems = filteredItems.OrderBy(item => item.Amount).ToList();
                break;
            case "title":
                filteredItems = filteredItems.OrderBy(item => item.Title).ToList();
                break;
            default:
                Console.WriteLine("Invalid sorting criteria. Sorting by month by default.");
                filteredItems = filteredItems.OrderBy(item => DateTime.ParseExact(item.Month, "MMMM", CultureInfo.CurrentCulture).Month).ToList();
                break;
        }

        // Display filtered and sorted items
        foreach (var item in filteredItems)
        {
            // Determine color based on type
            ConsoleColor color = ConsoleColor.White;
            if (item.Type == "Expense")
            {
                color = ConsoleColor.Red;
                totalExpense += item.Amount; // Increment total expense
            }
            else if (item.Type == "Income")
            {
                color = ConsoleColor.Green;
                totalIncome += item.Amount; // Increment total income
            }

            // Set color and print item details with padding
            Console.ForegroundColor = color;
            Console.WriteLine($"{item.Title.PadRight(titleWidth)} {item.Amount.ToString().PadRight(amountWidth)} {item.Month.PadRight(monthWidth)} {item.Type.PadRight(typeWidth)}");
        }

        // Reset color after printing items
        Console.ResetColor();

        // Print total income and total expense with color
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Income: {totalIncome.ToString().PadRight(20)}");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Total Expense: {totalExpense.ToString().PadRight(20)}");
        Console.ResetColor();
    }

    // Method to add item to the list
    public void AddItem(string title, double amount, string month, string itemType)
    {
        // Add the new item to the list
        items.Add(new MoneyItem(title, amount, month, itemType));

        // Save data to the fixed file path
        SaveToFile();

        // Display success message
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Item added successfully.");
        Console.ResetColor();
    }

    // Method to edit item in the list
    public void EditItem(int index, string title, double amount, string month, string itemType)
    {
        // Edit the item
        if (index >= 0 && index < items.Count)
        {
            items[index] = new MoneyItem(title, amount, month, itemType);

            // Save data to the fixed file path
            SaveToFile();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Item edited successfully.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }

    // Method to remove item from the list
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);

            // Save data to the fixed file path
            SaveToFile();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Item removed successfully.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Invalid index.");
        }
    }

    // Method to quit the application
    public void Quit()
    {
        // Save data to the fixed file path before quitting
        SaveToFile();
        Console.WriteLine("Quitting...");
    }

    // Method to save data to a text file
    private void SaveToFile()
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var item in items)
            {
                writer.WriteLine($"{item.Title},{item.Amount},{item.Month},{item.Type}");
            }
        }
    }

    // Method to load data from a text file
    public void LoadFromFile()
    {
        if (File.Exists(filePath))
        {
            items.Clear(); // Clear existing items before loading from file
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        string title = parts[0];
                        double amount = double.Parse(parts[1]);
                        string month = parts[2];
                        string type = parts[3];
                        items.Add(new MoneyItem(title, amount, month, type));
                    }
                }
            }
            Console.WriteLine("Data loaded successfully.");
        }
        else
        {
            Console.WriteLine("No previous data found.");
        }
    }
}

// Main class to run the money tracking application
class Program
{
    static void Main(string[] args)
    {
        MoneyTracker tracker = new MoneyTracker();

        Console.WriteLine("Welcome to CashFlow Tracker");

        // Main menu loop
        while (true)
        {
            Console.WriteLine("Pick an option:");
            Console.WriteLine("(1) Show items (ALL/Expense/Income)");
            Console.WriteLine("(2) Add New Expense/Income");
            Console.WriteLine("(3) Edit Item");
            Console.WriteLine("(4) Remove Item");
            Console.WriteLine("(5) Quit");

            string choice = Console.ReadLine();

            // Switch based on user choice
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter item type (ALL/Expense/Income):");
                    string itemType = Console.ReadLine().ToUpper();
                    Console.WriteLine("Enter sorting type (month/amount/title):");
                    string sortBy = Console.ReadLine().ToLower();
                    tracker.ShowItemList(itemType, sortBy);
                    break;
                case "2":
                    Console.WriteLine("Enter title:");
                    string title = Console.ReadLine();
                    Console.WriteLine("Enter amount:");
                    double amount = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter month:");
                    string month = Console.ReadLine();
                    Console.WriteLine("Enter type (Expense/Income):");
                    string type = Console.ReadLine().ToUpper();
                    tracker.AddItem(title, amount, month, type);
                    break;
                case "3":
                    Console.WriteLine("Enter the index of the item to edit:");
                    int editIndex = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter new title:");
                    string newTitle = Console.ReadLine();
                    Console.WriteLine("Enter new amount:");
                    double newAmount = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Enter new month:");
                    string newMonth = Console.ReadLine();
                    Console.WriteLine("Enter new type (Expense/Income):");
                    string newType = Console.ReadLine().ToUpper();
                    tracker.EditItem(editIndex, newTitle, newAmount, newMonth, newType);
                    break;
                case "4":
                    Console.WriteLine("Enter the index of the item to remove:");
                    int removeIndex = Convert.ToInt32(Console.ReadLine());
                    tracker.RemoveItem(removeIndex);
                    break;
                case "5":
                    tracker.Quit();
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
}
