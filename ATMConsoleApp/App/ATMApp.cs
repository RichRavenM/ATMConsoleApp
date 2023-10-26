using ATMConsoleApp.UI;
using System;

namespace ATMConsoleApp
{
    class ATMApp
    {
        static void Main(string[] args)
        {
            AppScreen.Welcome();
            long cardNumber = Validator.Convert<long>("your card number");
            Console.WriteLine($"Your card number is {cardNumber}");
            Utility.PressEnterToContinue();
        }
    }
}