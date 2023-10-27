using ATMConsoleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.UI
{
    public class AppScreen
    {
        internal const string currency = "£";
        internal static void Welcome()
        {
            //clears the console screen
            Console.Clear();

            //set the title of the console window
            Console.Title = "My ATM Console App";

            //sets the text color to white
            Console.ForegroundColor = ConsoleColor.White;

            //set the welcome message
            Console.WriteLine("\n\n--------------------Welcome to my ATM Console App-------------------\n");
            //prompt the user to insert their card
            Console.WriteLine("Please insert your bank card");
            Console.WriteLine("Note: Actual ATM will accept, read the card number of, and validate physical bank card");
            Utility.PressEnterToContinue();
        }

        internal static UserAccount UserLoginForm()
        {
            UserAccount tempUserAccount = new UserAccount();
            tempUserAccount.CardNumber = Validator.Convert<long>("your card number");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your PIN:"));
            return tempUserAccount;
        }

        internal static void LoginProcess()
        {
            Console.WriteLine("\nWe are checking your card number and pin...");
            Utility.PrintDotAnimation(10);
        }

        internal static void PrintLockScreen()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please visit your local branch to unlock your account.", false);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome back {fullName}");
            Utility.PressEnterToContinue();
        }

        internal static void DisplayAppMenu()
        {
            Console.Clear();
            Console.WriteLine("------- My ATM App Menu -------");
            Console.WriteLine(":                             :");
            Console.WriteLine("1. Account Balance            :");
            Console.WriteLine("2. Deposit Money              :");
            Console.WriteLine("3. Withdraw Money             :");
            Console.WriteLine("4. Transfer Money             :");
            Console.WriteLine("5. Transactions               :");
            Console.WriteLine("6. Logout                     :");
        }

        internal static void LogoutProgress()
        {
            Console.WriteLine("Thank you for using this ATM Console app");
            Utility.PrintDotAnimation(10);
            Console.Clear();
        }

        internal static int SelectAmount()
        {
            Console.WriteLine();
            Console.WriteLine(@"1: {0}5                 4:{0}50 ", currency);
            Console.WriteLine(@"2: {0}10                5:{0}100", currency);
            Console.WriteLine(@"3: {0}20                6:{0}200", currency);
            Console.WriteLine("0: Other");
            Console.WriteLine();

            int selectedAmount = Validator.Convert<int>("option:");

            switch (selectedAmount)
            {
                case 1:
                    return 5;
                case 2:
                    return 10;
                case 3:
                    return 20;
                case 4:
                    return 50;
                case 5:
                    return 100;
                case 6:
                    return 200;
                case 0:
                    return 0;
                default:
                    Utility.PrintMessage("Invalid input. Please try again.", false);
                    SelectAmount();
                    return -1;
            }
        }

        internal InternalTransfer InternalTransferForm() 
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.RecipientBankAccoutNumber = Validator.Convert<long>("recipient's bank account number:");
            internalTransfer.RecipientBankAccountName = Utility.GetUserInput("recipient's name");
            internalTransfer.TransferAmount = Validator.Convert<decimal>($"amount {currency}:");

            return internalTransfer;
        }
    }
}
