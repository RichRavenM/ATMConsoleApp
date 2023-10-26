using ATMConsoleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMConsoleApp.UI
{
    public static class AppScreen
    {
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
    }
}
