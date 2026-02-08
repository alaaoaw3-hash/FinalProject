using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.Json;

namespace BuildingBlocks
{
    /* 
        "Item" and "Category" classes are defined to 
        properly receive JSON files' content 
    */
    public class Item
    {
        public string? name {get; set;}
        public decimal price {get; set;}
    }
    public class Category
    {
        public string? CategoryName {get; set;}
        public Item[] items { get; set; } = Array.Empty<Item>();
    }
    public class Transaction(int id, string state, decimal moneyAmount, DateTime date)
    {
        int transactionID = id;
        string? moneyState = state;      // that is, money comining in or going out
        decimal amount = moneyAmount;
        DateTime transactionDate = date;

        // properties
        public int TransactionID
        {
            get {return transactionID;}
        }
        public string MoneyState
        {
            get {return moneyState;}
        }
        public decimal Amount
        {
            get {return amount;}
        }
        public DateTime TransactionDate
        {
            get {return transactionDate;}
        }
    }

    public class Person(decimal initialBalance, string name, int age)
    {
        decimal balance = initialBalance;
        string name = name;
        int age = age;
        int t_id = 0;
        List<Transaction> transactionRecord = [];

        // Handles adding money process when requested
        public void HandleAddingMoney()
        {
            AddMoneyAgain:
            Console.Write("\nHow much do you want to add? ");
            if(!decimal.TryParse(Console.ReadLine(), out decimal moneyToAdd) || moneyToAdd < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou must give a valid amount to add\n");
                Console.ResetColor();
                goto AddMoneyAgain;
            }

            AddMoney(moneyToAdd);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Money added successfully.");
            Console.ResetColor();
            Console.Write($"Current Balance: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"YER {balance}\n");
            Console.ResetColor();
            RecordTransaction("moneyIn", moneyToAdd, DateTime.Now);
        }

        // Creates sample data to test graphing
        void GraphingTestData()
        {
            transactionRecord.Add(new Transaction(1, "moneyIn", 2500.00m, new DateTime(2024, 1, 1)));
            transactionRecord.Add(new Transaction(2, "moneyOut", 850.00m, new DateTime(2024, 1, 2)));
            transactionRecord.Add(new Transaction(3, "moneyOut", 120.50m, new DateTime(2024, 1, 5)));
            transactionRecord.Add(new Transaction(4, "moneyOut", 45.00m, new DateTime(2024, 1, 7)));
            transactionRecord.Add(new Transaction(5, "moneyOut", 200.00m, new DateTime(2024, 1, 10)));
            transactionRecord.Add(new Transaction(6, "moneyIn", 2500.00m, new DateTime(2024, 2, 1)));
            transactionRecord.Add(new Transaction(7, "moneyOut", 850.00m, new DateTime(2024, 2, 2)));
            transactionRecord.Add(new Transaction(8, "moneyOut", 95.00m, new DateTime(2024, 2, 5)));
            transactionRecord.Add(new Transaction(9, "moneyOut", 60.00m, new DateTime(2024, 2, 8)));
            transactionRecord.Add(new Transaction(10, "moneyOut", 150.00m, new DateTime(2024, 2, 14)));
            transactionRecord.Add(new Transaction(11, "moneyOut", 300.00m, new DateTime(2024, 2, 20)));
            transactionRecord.Add(new Transaction(12, "moneyIn", 2600.00m, new DateTime(2024, 3, 1)));
            transactionRecord.Add(new Transaction(13, "moneyOut", 875.00m, new DateTime(2024, 3, 2)));
            transactionRecord.Add(new Transaction(14, "moneyOut", 110.00m, new DateTime(2024, 3, 5)));
            transactionRecord.Add(new Transaction(15, "moneyOut", 55.00m, new DateTime(2024, 3, 8)));
            transactionRecord.Add(new Transaction(16, "moneyOut", 180.00m, new DateTime(2024, 3, 12)));
            transactionRecord.Add(new Transaction(17, "moneyOut", 75.00m, new DateTime(2024, 3, 15)));
            transactionRecord.Add(new Transaction(18, "moneyOut", 220.00m, new DateTime(2024, 3, 18)));
            transactionRecord.Add(new Transaction(19, "moneyOut", 40.00m, new DateTime(2024, 3, 22)));
            transactionRecord.Add(new Transaction(20, "moneyOut", 500.00m, new DateTime(2024, 3, 25)));
        }

        public void ViewBalance()
        {
            Console.Write("Current balance: ");
            if(balance > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"YER {balance}");
                Console.ResetColor();
            }
        }

        public bool AddMoney(decimal moneyIn)
        {
            if(moneyIn < 0)
            {
                return false;
            }

            balance += moneyIn;
            return true;
        }

        public bool TakeMoneyOut(decimal moneyOut)
        {
            if(moneyOut < 0 || moneyOut > balance)
            {
                return false;
            }

            balance -= moneyOut;
            Console.WriteLine($"Current Balance: {balance}");
            return true;
        }

        void RecordTransaction(string state, decimal moneyAmount, DateTime date)
        {
            var newTransaction = new Transaction(t_id, state, moneyAmount, date);
            transactionRecord.Add(newTransaction);
            ++t_id;
        }

        void ShowItems(Category JSON_Object)
        {
            // Show available items
            int itemNumber = 1;
            Console.WriteLine("What item do you want to buy?\n");
            if (JSON_Object.items != null)
            {
                for(int i = 0; i < JSON_Object.items.Length; i++)
                {
                    var item_1 = JSON_Object.items[i];
                    var item_2 = JSON_Object.items[i+1];
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"{itemNumber}- {item_1.name}:");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" YER {item_1.price}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"\t\t{++itemNumber}- {item_2.name}:");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" YER {item_2.price}\n");
                    ++itemNumber;
                    ++i;
                }
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("No items available in this category.");
            }
        }

        void ShowReceipt(Item item)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{item.name}: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"YER {item.price}");
            Console.ResetColor();
        }

        string CompareCostAndBalance(decimal cost)
        {
            if(cost > balance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You don't have enough balance");
                Thread.Sleep(1500);
                Console.WriteLine("You will prompted again to choose an item and amount.");
                Thread.Sleep(1500);
                Console.Write("\n");
                Console.ResetColor();
                return "larger";
            }
            else if(cost == balance)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("The cost will consume all of your balance, are you sure you want to do the purchase? ");
                YesOrNo:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(Y / N) ");
                Console.ResetColor();
                if(!char.TryParse(Console.ReadLine(), out char confirmation) || !"yn".Contains(char.ToLower(confirmation)))
                {
                    Console.WriteLine("You must either choose Y or N");
                    goto YesOrNo;
                }

                if(char.ToLower(confirmation) == 'y')
                {
                    TakeMoneyOut(cost);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Purchase is successfull");
                    Console.ResetColor();
                    RecordTransaction("moneyOut", cost, DateTime.Now);
                    return "finished buying";
                }
                else if (char.ToLower(confirmation) == 'n')
                {
                    return "not confirmed";
                } 
            }
            else
            {
                TakeMoneyOut(cost);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Purchase is successfull");
                Console.ResetColor();
                RecordTransaction("moneyOut", cost, DateTime.Now);
                return "less";
            }

            // Ensure all code paths return a value
            return "unknown";
        }

        public void Buy()
        {
            char chosenCategory;
            string availableCharacters = "abcde";
            bool didFinishBuying = false;
            while (!didFinishBuying)
            {
                Console.WriteLine("\nWhat is the category of the thing you want to buy?");
                Console.WriteLine("Available categories: A- Groceries, B- Clothing, C- Electronics, D- Health & beauty, E- Home & Kitchen");
                Console.Write("(Choose one of the characters): ");
                if(!char.TryParse(Console.ReadLine(), out chosenCategory) || !availableCharacters.Contains(char.ToLower(chosenCategory)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPlease only choose one of the available characters\n");
                    Console.ResetColor();
                    continue;
                }

                string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                string filePath = string.Empty;
                switch (char.ToLower(chosenCategory))
                {
                    case 'a':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Groceries.json");
                        break;
                    case 'b':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Clothing.json");
                        break;
                    case 'c':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Electronics.json");
                        break;
                    case 'd':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Health_&_Beauty.json");
                        break;
                    case 'e':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Home_&_Kitchen.json");
                        break;
                }
                
                // Fetch the corresponding JSON file contents
                string JSON_String = File.ReadAllText(filePath);
                Category? chosenCategoryAndItsItems = JsonSerializer.Deserialize<Category>(JSON_String);

                WhenBalanceIsNotEnough:
                ShowItems(chosenCategoryAndItsItems);

                // Allowing user to choose an item, and rejecting invalid inputs
                ChooseItemAgain:
                Console.Write("(Choose an item by choosing a number): ");
                if(!int.TryParse(Console.ReadLine(), out int chosenItemNumber) || chosenItemNumber < 1 || chosenItemNumber > 20)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nYou must choose a number that corresponds to one of the available items.\n");
                    Console.ResetColor();
                    goto ChooseItemAgain;
                }

                var chosenItem = chosenCategoryAndItsItems.items[chosenItemNumber - 1];

                ShowReceipt(chosenItem);
                
                ChooseAmountAgain:
                Console.Write("Amount: ");
                if(!int.TryParse(Console.ReadLine(), out int amountOfItem))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nAmount should be a valid whole number.\n");
                    Console.ResetColor();
                    goto ChooseAmountAgain;
                }

                // calculate cost
                decimal cost = amountOfItem * chosenItem.price;
                string cost_balanceCheck = CompareCostAndBalance(cost);

                // Can the user buy the item?
                if(cost_balanceCheck == "larger")
                {
                    goto WhenBalanceIsNotEnough;
                }
                else if(cost_balanceCheck == "finished buying")
                {
                    didFinishBuying = true;
                }
                else if(cost_balanceCheck == "not confirmed")
                {
                    goto WhenBalanceIsNotEnough;
                }
                else if(cost_balanceCheck == "less")
                {
                    didFinishBuying = true;
                }
            }


        }
        
    }
}