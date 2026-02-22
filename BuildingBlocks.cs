using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Text.Json;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

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
        // this class resembles a JSON file of the items
        public string? CategoryName {get; set;}
        public Item[] items { get; set; } = Array.Empty<Item>();
    }
    public class Transaction(int id, string state, string? category, decimal moneyAmount, DateTime date)
    {
        int transactionID = id;
        string? moneyState = state;  
        string? category = category;    // that is, money comining in or going out
        decimal amount = moneyAmount;
        DateTime transactionDate = date;

        // properties
        public int TransactionID
        {
            get {return transactionID;}
        }
        public string MoneyState
        {
            get {return moneyState!;}
        }
        public string Category
        {
            get {return category!;}
        }
        public decimal Amount
        {
            get {return amount;}
        }
        public DateTime TransactionDate
        {
            get {return transactionDate;}
        }

        public override string ToString()
        {
            return $"TransactionID: {TransactionID}, moneyState: {MoneyState}, Category: {Category}, Amount: {Amount}, Date: {TransactionDate}";
        }
    }
    class SpendingSummary
    {
        // Spending sums
        public decimal GroceriesSum;
        public decimal ClothingSum;
        public decimal ElectronicsSum;
        public decimal Health_and_BeautySum;
        public decimal Home_and_KitchenSum;

        // Percentages
        public double GroceriesPercentage;
        public double ClothingPercentage;
        public double ElectronicsPercentage;
        public double Health_and_BeautyPercentage;
        public double Home_and_KitchenPercentage;

        public override string ToString()
        {
            return $"""
                Groceries Sum: YER {GroceriesSum}      Groceries Percentage: {GroceriesPercentage:F1}%
                Clothing Sum: YER {ClothingSum}        Clothing Percentage: {ClothingPercentage:F1}%
                Electronics Sum: YER {ElectronicsSum}  Electronics Percentage: {ElectronicsPercentage:F1}%
                Health & Beauty Sum: YER {Health_and_BeautySum}    Health & Beauty Percentage: {Health_and_BeautyPercentage:F1}%
                Home & Kitchen Sum: YER {Home_and_KitchenSum}      Home & Kitchen Percentage: {Home_and_KitchenPercentage:F1}%
                """;
        }                
    }
    public class Person(decimal initialBalance, string name, int age)
    {
        decimal initial_balance = initialBalance;
        decimal current_balance = initialBalance;
        string name = name;
        int age = age;
        int t_id = 0;
        List<Transaction> transactionRecord = [];

        // static bool __transactionRecordPopulated = false;

        /* ---- Public methods ---- */
        public void ViewBalance()
        {
            Console.Write("Current balance: ");
            if(current_balance > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"YER {current_balance}");
                Console.ResetColor();
            }
        }
        public void PlotTheGraphs()
        {
            string[] categoriesNames = ["Groceries", "Clothing", "Electronics", "Health_&_Beauty", "Home_&_Kitchen"];
            SpendingSummary ss_object = CalculateSumsAndPercentages();
            Console.WriteLine("\n----- Spendings Graphs -----\n");
            Console.WriteLine($"Base balance: YER {initial_balance}");
            for(int i = 0; i < categoriesNames.Length; i++)
            {
                // Calculate the number of boxes to print for category in intrest
                int numberOfBoxes = CalculateNumberOfBoxes(categoriesNames[i], ss_object);
                
                Console.Write($"{categoriesNames[i]}\t|");
                // print out the boxes
                for(int box = 0; box < numberOfBoxes; box++)
                {
                    Console.Write("█");
                }

                PrintSumAndPercentage(categoriesNames[i], ss_object);
                Console.Write("\n");
            }
        }
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
            initial_balance = current_balance;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Money added successfully.");
            Console.ResetColor();
            Console.Write($"Current Balance: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"YER {current_balance}\n");
            Console.ResetColor();
            RecordTransaction("moneyIn", "income", moneyToAdd, DateTime.Now);
        }
        public void Buy()
        {
            char chosenCategory;
            string availableCharacters = "abcde";
            bool didFinishBuying = false;
            while (!didFinishBuying)
            {
                Console.WriteLine("\nWhat is the category you want to buy from?");
                Console.WriteLine("Available categories: A- Groceries, B- Clothing, C- Electronics, D- Health & beauty, E- Home & Kitchen");
                Console.Write("(Choose one of the characters): ");
                if(!char.TryParse(Console.ReadLine(), out chosenCategory) || !availableCharacters.Contains(char.ToLower(chosenCategory)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPlease only choose one of the available characters\n");
                    Console.ResetColor();
                    continue;
                }

                string chosenCategoryString = string.Empty;
                string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
                string filePath = string.Empty;
                switch (char.ToLower(chosenCategory))
                {
                    case 'a':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Groceries.json");
                        chosenCategoryString = "Groceries";
                        break;
                    case 'b':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Clothing.json");
                        chosenCategoryString = "Clothing";
                        break;
                    case 'c':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Electronics.json");
                        chosenCategoryString = "Electronics";
                        break;
                    case 'd':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Health_&_Beauty.json");
                        chosenCategoryString = "Health_&_Beauty";
                        break;
                    case 'e':
                        filePath = Path.Combine(basePath, "Buying_Categories", "Home_&_Kitchen.json");
                        chosenCategoryString = "Home_&_Kitchen";
                        break;
                }
                
                // Fetch the corresponding JSON file contents
                string JSON_String = File.ReadAllText(filePath);
                Category? chosenCategoryAndItsItems = JsonSerializer.Deserialize<Category>(JSON_String);

                WhenBalanceIsNotEnough:
                ShowItems(chosenCategoryAndItsItems!);

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

                var chosenItem = chosenCategoryAndItsItems?.items[chosenItemNumber - 1];

                ShowReceipt(chosenItem!);
                
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
                decimal cost = amountOfItem * chosenItem!.price;
                string cost_balanceCheck = CompareCostAndBalance(cost, chosenCategoryString);

                // Can the user buy the item?
                if(cost_balanceCheck == "larger" || cost_balanceCheck == "not confirmed")
                {
                    goto WhenBalanceIsNotEnough;
                }
                else if(cost_balanceCheck == "finished buying" || cost_balanceCheck == "less")
                {
                    didFinishBuying = true;
                }
            }


        }
        public void PrintOutTransactions()
        {   
            if(transactionRecord.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nTransaction record is empty.\n");
                Console.ResetColor();
                return;
            }

            // print out the transactions
            Console.Write("\n");
            foreach(var transaction in transactionRecord)
            {
                Console.WriteLine(transaction);
            }
        }

        /* ---- Helper Methods (all are private) ----*/

        // Creates sample data to test graphing
        void GraphingTestData()
        {
            transactionRecord.Add(new Transaction(1, "moneyIn", "income", 2500.00m, new DateTime(2024, 1, 1)));
            transactionRecord.Add(new Transaction(2, "moneyOut", "Home_&_Kitchen", 850.00m, new DateTime(2024, 1, 2)));
            transactionRecord.Add(new Transaction(3, "moneyOut", "Groceries", 120.50m, new DateTime(2024, 1, 5)));
            transactionRecord.Add(new Transaction(4, "moneyOut", "Groceries", 45.00m, new DateTime(2024, 1, 7)));
            transactionRecord.Add(new Transaction(5, "moneyOut", "Health_&_Beauty", 35.00m, new DateTime(2024, 1, 8)));
            transactionRecord.Add(new Transaction(6, "moneyOut", "Clothing", 75.00m, new DateTime(2024, 1, 12)));
            transactionRecord.Add(new Transaction(7, "moneyOut", "Electronics", 200.00m, new DateTime(2024, 1, 15)));
            transactionRecord.Add(new Transaction(8, "moneyIn", "income", 2500.00m, new DateTime(2024, 2, 1)));
            transactionRecord.Add(new Transaction(9, "moneyOut", "Home_&_Kitchen", 850.00m, new DateTime(2024, 2, 2)));
            transactionRecord.Add(new Transaction(10, "moneyOut", "Groceries", 95.00m, new DateTime(2024, 2, 5)));
            transactionRecord.Add(new Transaction(11, "moneyOut", "Groceries", 60.00m, new DateTime(2024, 2, 8)));
            transactionRecord.Add(new Transaction(12, "moneyOut", "Health_&_Beauty", 40.00m, new DateTime(2024, 2, 10)));
            transactionRecord.Add(new Transaction(13, "moneyOut", "Clothing", 110.00m, new DateTime(2024, 2, 16)));
            transactionRecord.Add(new Transaction(14, "moneyOut", "Electronics", 150.00m, new DateTime(2024, 2, 20)));
            transactionRecord.Add(new Transaction(15, "moneyOut", "Groceries", 85.00m, new DateTime(2024, 2, 25)));
            transactionRecord.Add(new Transaction(16, "moneyIn", "income", 2600.00m, new DateTime(2024, 3, 1)));
            transactionRecord.Add(new Transaction(17, "moneyOut", "Home_&_Kitchen", 875.00m, new DateTime(2024, 3, 2)));
            transactionRecord.Add(new Transaction(18, "moneyOut", "Groceries", 110.00m, new DateTime(2024, 3, 5)));
            transactionRecord.Add(new Transaction(19, "moneyOut", "Groceries", 55.00m, new DateTime(2024, 3, 8)));
            transactionRecord.Add(new Transaction(20, "moneyOut", "Electronics", 450.00m, new DateTime(2024, 3, 18)));
        }
        SpendingSummary CalculateSumsAndPercentages()
        {
            // Variables that will hold spendings sum
            decimal Groceries = 0;
            decimal Clothing = 0;
            decimal Electronics = 0;
            decimal Health_and_Beauty = 0;
            decimal Home_and_Kitchen = 0;

            // Variables that will hold categories' percentages
            double GroceriesPercentage = 0;
            double ClothingPercentage = 0;
            double ElectronicsPercentage = 0;
            double Health_and_BeautyPercentage = 0;
            double Home_and_KitchenPercentage = 0;

            // figure out last money addition
            int startingTransaction = 0;
            for(int i = transactionRecord.Count - 1; i > 0; i--)
            {
                if(transactionRecord[i].MoneyState == "moneyIn")
                {
                    startingTransaction = i;
                    break;
                }
            }

            // Summing spendings
            for(int i = startingTransaction; i < transactionRecord.Count; i++)
            {
                if(transactionRecord[i].MoneyState == "moneyOut" && transactionRecord[i].Category == "Groceries")
                {
                    Groceries += transactionRecord[i].Amount;
                }
                if(transactionRecord[i].MoneyState == "moneyOut" && transactionRecord[i].Category == "Clothing")
                {
                    Clothing += transactionRecord[i].Amount;
                }
                if(transactionRecord[i].MoneyState == "moneyOut" && transactionRecord[i].Category == "Electronics")
                {
                    Electronics += transactionRecord[i].Amount;
                }
                if(transactionRecord[i].MoneyState == "moneyOut" && transactionRecord[i].Category == "Health_&_Beauty")
                {
                    Health_and_Beauty += transactionRecord[i].Amount;
                }
                if(transactionRecord[i].MoneyState == "moneyOut" && transactionRecord[i].Category == "Home_&_Kitchen")
                {
                    Home_and_Kitchen += transactionRecord[i].Amount;
                }
            }

            // Calculating percentages
            GroceriesPercentage = Convert.ToDouble((Groceries / initial_balance) * 100);
            ClothingPercentage = Convert.ToDouble((Clothing / initial_balance) * 100);
            ElectronicsPercentage = Convert.ToDouble((Electronics / initial_balance) * 100);
            Health_and_BeautyPercentage = Convert.ToDouble((Health_and_Beauty / initial_balance) * 100);
            Home_and_KitchenPercentage = Convert.ToDouble((Home_and_Kitchen / initial_balance) * 100);

            // storing all values in an instance of SpendingSummary
            SpendingSummary ss_object = new()
            {
                GroceriesSum = Groceries,
                ClothingSum = Clothing,
                ElectronicsSum = Electronics,
                Health_and_BeautySum = Health_and_Beauty,
                Home_and_KitchenSum = Home_and_Kitchen,

                GroceriesPercentage = GroceriesPercentage,
                ClothingPercentage = ClothingPercentage,
                ElectronicsPercentage = ElectronicsPercentage,
                Health_and_BeautyPercentage = Health_and_BeautyPercentage,
                Home_and_KitchenPercentage = Home_and_KitchenPercentage
            };

            return ss_object;
        }
        int CalculateNumberOfBoxes(string categoryName, SpendingSummary ss_object)
        {
            int numberOfBoxes = 0;
            if(categoryName == "Groceries")
                {
                    numberOfBoxes = Convert.ToInt32(Math.Floor(ss_object.GroceriesPercentage / 10));
                    if(numberOfBoxes == 0 && ss_object.GroceriesPercentage > 0)
                    {
                        numberOfBoxes = 1;
                    }
                }
                else if(categoryName == "Clothing")
                {
                    numberOfBoxes = Convert.ToInt32(Math.Floor(ss_object.ClothingPercentage / 10));
                    if(numberOfBoxes == 0 && ss_object.ClothingPercentage > 0)
                    {
                        numberOfBoxes = 1;
                    }
                }
                else if(categoryName == "Electronics")
                {
                    numberOfBoxes = Convert.ToInt32(Math.Floor(ss_object.ElectronicsPercentage / 10));
                    if(numberOfBoxes == 0 && ss_object.ElectronicsPercentage > 0)
                    {
                        numberOfBoxes = 1;
                    }
                }
                else if(categoryName == "Health_&_Beauty")
                {
                    numberOfBoxes = Convert.ToInt32(Math.Floor(ss_object.Health_and_BeautyPercentage / 10));
                    if(numberOfBoxes == 0 && ss_object.Health_and_BeautyPercentage > 0)
                    {
                        numberOfBoxes = 1;
                    }
                }
                else if(categoryName == "Home_&_Kitchen")
                {
                    numberOfBoxes = Convert.ToInt32(Math.Floor(ss_object.Home_and_KitchenPercentage / 10));
                    if(numberOfBoxes == 0 && ss_object.Home_and_KitchenPercentage > 0)
                    {
                        numberOfBoxes = 1;
                    }
                }

                // defensive condition to prevent "numberOfBoxes" from ever going beyond 10 
                if(numberOfBoxes > 10){ numberOfBoxes = 10; }
                else if(numberOfBoxes < 0){ numberOfBoxes = 0; }

                return numberOfBoxes;
        }
        void PrintSumAndPercentage(string categoryName ,SpendingSummary ss_object)
        {
            if(categoryName == "Groceries")
            {
                Console.Write($"\t YER {ss_object.GroceriesSum}, {ss_object.GroceriesPercentage:F1}%");
            }
            else if(categoryName == "Clothing")
            {
                Console.Write($"\t YER {ss_object.ClothingSum}, {ss_object.ClothingPercentage:F1}%");
            }
            else if(categoryName == "Electronics")
            {
                Console.Write($"\t YER {ss_object.ElectronicsSum}, {ss_object.ElectronicsPercentage:F1}%");
            }
            else if(categoryName == "Health_&_Beauty")
            {
                Console.Write($"\t YER {ss_object.Health_and_BeautySum}, {ss_object.Health_and_BeautyPercentage:F1}%");
            }
            else if(categoryName == "Home_&_Kitchen")
            {
                Console.Write($"\t YER {ss_object.Home_and_KitchenSum}, {ss_object.Health_and_BeautyPercentage:F1}%");
            }
        }
        bool AddMoney(decimal moneyIn)
        {
            if(moneyIn < 0)
            {
                return false;
            }

            current_balance += moneyIn;
            return true;
        }
        bool TakeMoneyOut(decimal moneyOut)
        {
            if(moneyOut < 0 || moneyOut > current_balance)
            {
                return false;
            }

            current_balance -= moneyOut;
            Console.WriteLine($"Current Balance: YER {current_balance}");
            return true;
        }
        void RecordTransaction(string state, string category, decimal moneyAmount, DateTime date)
        {
            var newTransaction = new Transaction(t_id, state, category, moneyAmount, date);
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
        string CompareCostAndBalance(decimal cost, string category)
        {
            if(cost > current_balance)
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
            else if(cost == current_balance)
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
                    RecordTransaction("moneyOut", category, cost, DateTime.Now);
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
                RecordTransaction("moneyOut", category, cost, DateTime.Now);
                return "less";
            }

            // Ensure all code paths return a value
            return "unknown";
        }
        
        // Develper Methods
        public void __PrintOutSpendingSummary()
        {
            SpendingSummary speandingSummary = CalculateSumsAndPercentages();
            Console.WriteLine(speandingSummary);
        }
    }
}