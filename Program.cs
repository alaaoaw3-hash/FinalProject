using System.Linq;
using BuildingBlocks;

namespace Program
{
    class PersonalFinanceManager
    {
        static void Main()
        {
            Console.WriteLine("\n");
            Console.WriteLine("     ╔══════════════════════════════╗");
            Console.WriteLine("     ║   Personal Finance Manager   ║");
            Console.WriteLine("     ╚══════════════════════════════╝");
            Console.WriteLine("\n");

                    /* ---- Prompt user for essential information ---- */

            string? userName = null;
            int userAge = 0;
            decimal userInitialBalance = 0;

            // Flow control flags        
            bool hasName = false;
            bool isValidNumber = false;
            bool isValidAge = false;

            while(!hasName || !isValidNumber || !isValidAge)
            {
                // Enforce valid name input
                if (!hasName)
                {
                    Console.Write("What is your name? ");
                    userName = Console.ReadLine();
                    if (string.IsNullOrEmpty(userName))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYou must provide a valid name.\n");
                        Console.ResetColor();
                        continue;
                    }
                    else if (userName.Any(char.IsDigit))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nName cannot contain numbers.\n");
                        Console.ResetColor();
                        continue;
                    }
                    hasName = true;
                }

                // Enforce valid age
                if (!isValidAge)
                {
                    Console.Write("How old are you? ");
                    if(!int.TryParse(Console.ReadLine(), out userAge))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYou must provide a valid age.\n");
                        Console.ResetColor();
                        continue;
                    }
                    else if(userAge < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nAge cannot be zero.\n");
                        Console.ResetColor();
                        continue;
                    }
                    else if(userAge > 120)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nAge is too high\n");
                        Console.ResetColor();
                        continue;
                    }
                    isValidAge = true;
                }

                // Enforce valid balance input
                if (!isValidNumber)
                {
                    Console.Write("What is your current balance? ");
                    if(!decimal.TryParse(Console.ReadLine(), out userInitialBalance))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYou must provide a valid number for the balance.\n");
                        Console.ResetColor();
                        continue;
                    }
                    else if(userInitialBalance < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInitial balance cannot be below zero.\n");
                        Console.ResetColor();
                        continue;   
                    }
                    isValidNumber = true;
                }
            } 

            // Constuct the user object
            Person user = new(userInitialBalance, userName ?? "null", userAge);
            
            // Start the app
            bool isUsageShown = false;
            bool stop = false;
            while (!stop)
            {
                if (!isUsageShown)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nUsage: Write the number of the option you want proceed with.");
                    Console.ResetColor();
                    isUsageShown = true;
                }
                Console.WriteLine("\n1- View Balance\n2- Graph Spendings\n3- Add Money\n4- Buy\n5- Exit program");

                if(!int.TryParse(Console.ReadLine(), out int chosenOption) || chosenOption < 1 || chosenOption > 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nYou may choose one number of the available options.\n");
                    Console.ResetColor();
                    continue;
                }

                switch (chosenOption)
                {
                    case 1:
                        user.ViewBalance();
                        break;
                    case 2:
                        user.PlotTheGraphs();
                        break;
                    case 3:
                        user.HandleAddingMoney();
                        break;
                    case 4:
                        user.Buy();
                        break;
                    case 5:
                        stop = true;
                        break;

                    // developer options    
                    case 6:
                        user.__PrintOutTransactions();
                        break; 
                    case 7:
                        user.__PrintOutSpendingSummary();
                        break;       
                }        
            }
        }
    }
}