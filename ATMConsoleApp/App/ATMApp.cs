using ATMConsoleApp.Domain.Entities;
using ATMConsoleApp.Domain.Interfaces;
using ATMConsoleApp.UI;
using System;

namespace ATMConsoleApp
{
    public class ATMApp : IUserLogin
    {
        private List<UserAccount> userAccounts;
        private UserAccount selectedAccount;

        public void InitialiseData()
        {
            userAccounts = new List<UserAccount>
            {
                new UserAccount{ Id = 1, FullName = "RichM", AccountNumber = 12345678, CardNumber = 1234567812345678, CardPin = 1234, AccountBalance = 1000.00m, IsLocked = false },
                new UserAccount{ Id = 2, FullName = "DaveS", AccountNumber = 56781234, CardNumber = 5678123456781234, CardPin = 4321, AccountBalance = 100.00m, IsLocked = false },
                new UserAccount{ Id = 3, FullName = "TomT", AccountNumber = 78123456, CardNumber = 7812345678123456, CardPin = 3214, AccountBalance = 20000.00m, IsLocked = false }
            };
        }

        public void CheckUserCardNumberAndPassword()
        {
            bool isCorrectLogin = false;

            UserAccount tempUserAccount = new UserAccount();
            tempUserAccount.CardNumber = Validator.Convert<long>("your card number");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your PIN:"));

            Console.WriteLine("We are checking your card number and pin...");
            int timer = 10;
            for (int i = 0; i < timer; i++)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            Console.Clear();

        }

    }
}