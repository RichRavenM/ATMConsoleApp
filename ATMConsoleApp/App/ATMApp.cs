using ATMConsoleApp.Domain.Entities;
using ATMConsoleApp.Domain.Enums;
using ATMConsoleApp.Domain.Interfaces;
using ATMConsoleApp.UI;
using System;

namespace ATMConsoleApp
{
    public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccounts;
        private UserAccount selectedAccount;
        private List<Transaction> _transactionList;

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumberAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            AppScreen.DisplayAppMenu();
            ProcessMenuOption();
        }
        public void InitialiseData()
        {
            userAccounts = new List<UserAccount>
            {
                new UserAccount{ Id = 1, FullName = "RichM", AccountNumber = 12345678, CardNumber = 1234567812345678, CardPin = 1234, AccountBalance = 1000.00m, IsLocked = false },
                new UserAccount{ Id = 2, FullName = "DaveS", AccountNumber = 56781234, CardNumber = 5678123456781234, CardPin = 4321, AccountBalance = 100.00m, IsLocked = false },
                new UserAccount{ Id = 3, FullName = "TomT", AccountNumber = 78123456, CardNumber = 7812345678123456, CardPin = 3214, AccountBalance = 20000.00m, IsLocked = false }
            };
            _transactionList = new List<Transaction>();
        }

        public void CheckUserCardNumberAndPassword()
        {
            bool isCorrectLogin = false;

            while (!isCorrectLogin)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProcess();
                foreach (UserAccount acc in userAccounts)
                {
                    selectedAccount = acc;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;
                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = acc;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                AppScreen.PrintLockScreen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\nInvalid card number or PIN", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockScreen();
                        }
                    }
                    Console.Clear();
                }
            }
        }

        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("an option:"))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawal:
                    Console.WriteLine("Making withdrawwal...");
                    break;
                case (int)AppMenu.InternalTransfer:
                    Console.WriteLine("Processing transfer...");
                    break;
                case (int)AppMenu.ViewTransaction:
                    Console.WriteLine("Viewing transactions...");
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect your card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid input.", false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"You account balance is {Utility.FormatCurrency(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\n Only multiple of £5 allowed.\n");
            var transactionAmount = Validator.Convert<int>($"amount: {AppScreen.currency}");

            //simulate counting
            Console.WriteLine("\n Checking and counting bank notes.");
            Utility.PrintDotAnimation(10);
            Console.WriteLine("");

            //some guard clause
            if (transactionAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than 0. Please try again.", false);
                return;
            }
            if (transactionAmount % 5 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount that is a multiple of 5", false);
            }

            if (!PreviewBanknoteCount(transactionAmount))
            {
                Utility.PrintMessage("We have cancelled your action", false);
                return;
            }
            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id,TransactionType.Deposit, transactionAmount, "");

            // update account balance
            selectedAccount.AccountBalance += transactionAmount;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatCurrency(transactionAmount)} was successful");


        }

        public void MakeWithdrawal()
        {
            throw new NotImplementedException();
        }

        private bool PreviewBanknoteCount(int amount)
        {
            int twentyNotesCount = amount / 20;
            int tenNotesCount = (amount % 20) / 10;
            int fiveNotesCount = (amount % 10) / 5;

            Console.WriteLine("\nSummary:");
            Console.WriteLine("-------");
            Console.WriteLine($"{AppScreen.currency}20 X {twentyNotesCount} = {20 * twentyNotesCount}");
            Console.WriteLine($"{AppScreen.currency}10 X {tenNotesCount} = {20 * tenNotesCount}");
            Console.WriteLine($"{AppScreen.currency}5 X {fiveNotesCount} = {20 * fiveNotesCount}");
            Console.WriteLine($"Total Amount: {Utility.FormatCurrency(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);
        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _transactionType, decimal _transactionAmount, string _description)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountId = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _transactionType,
                TransactionAmount = _transactionAmount,
                Description = _description
            };

            //add transaction object to the list
            _transactionList.Add(transaction);
        }

        public void ViewTransaction()
        {
            throw new NotImplementedException();
        }
    }
}