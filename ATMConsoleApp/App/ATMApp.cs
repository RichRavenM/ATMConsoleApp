﻿using ATMConsoleApp.Domain.Entities;
using ATMConsoleApp.Domain.Enums;
using ATMConsoleApp.Domain.Interfaces;
using ATMConsoleApp.UI;
using ConsoleTables;
using System;
using System.Linq;

namespace ATMConsoleApp
{
    public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
    {
        private List<UserAccount> userAccounts;
        private UserAccount selectedAccount;
        private List<Transaction> _transactionList;
        private const decimal minimumKeptAmount = 5;
        private readonly AppScreen screen;

        public ATMApp()
        {
            screen = new AppScreen();
        }

        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumberAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
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
                        Utility.PrintMessage("\nInvalid card number or PIN.", false);
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
                    MakeWithdrawal();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
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
            Console.WriteLine("\nOnly multiple of £5 allowed.\n");
            var transactionAmount = Validator.Convert<int>($"amount: {AppScreen.currency}");

            //simulate counting
            Console.WriteLine("\nChecking and counting bank notes.");
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
                Utility.PrintMessage($"Enter deposit amount that is a multiple of 5.", false);
                return;
            }

            if (!PreviewBanknoteCount(transactionAmount))
            {
                Utility.PrintMessage("We have cancelled your action", false);
                return;
            }
            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transactionAmount, "");

            // update account balance
            selectedAccount.AccountBalance += transactionAmount;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatCurrency(transactionAmount)} was successful.");


        }

        public void MakeWithdrawal()
        {
            var transactionAmount = 0;
            int selectedAmmount = AppScreen.SelectAmount();

            if (selectedAmmount == -1)
            {
                MakeWithdrawal();
                return;
            }
            else if (selectedAmmount != 0)
            {
                transactionAmount = selectedAmmount;
            }
            else
            {
                transactionAmount = Validator.Convert<int>($"amount {AppScreen.currency}");
            }

            //input validation
            if (transactionAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than 0. Please try again.", false);
                return;
            }
            if (transactionAmount % 5 != 0)
            {
                Utility.PrintMessage($"Enter deposit amount that is a multiple of 5.", false);
                return;
            }

            //Business logic validation
            if (transactionAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Insufficient funds. Your balance does not have enough money to withdraw {Utility.FormatCurrency(transactionAmount)}.", false);
                return;
            }

            if ((selectedAccount.AccountBalance - transactionAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have a minimum of {Utility.FormatCurrency(minimumKeptAmount)} after a withdrawal", false);
                return;
            }

            //Bind withdrawal details to transactions object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal, -transactionAmount, "");
            //update account balance
            selectedAccount.AccountBalance -= transactionAmount;
            //success message
            Utility.PrintMessage($"You have succesfully withdrawn {Utility.FormatCurrency(transactionAmount)}");
        }

        private bool PreviewBanknoteCount(int amount)
        {
            int twentyNotesCount = amount / 20;
            int tenNotesCount = (amount % 20) / 10;
            int fiveNotesCount = (amount % 10) / 5;

            Console.WriteLine("\nSummary:");
            Console.WriteLine("-------");
            Console.WriteLine($"{AppScreen.currency}20 X {twentyNotesCount} = {20 * twentyNotesCount}");
            Console.WriteLine($"{AppScreen.currency}10 X {tenNotesCount} = {10 * tenNotesCount}");
            Console.WriteLine($"{AppScreen.currency}5 X {fiveNotesCount} = {5 * fiveNotesCount}");
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
            var filteredTransactionList = _transactionList.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
            //check if there's a transaction
            if (filteredTransactionList.Count < 1)
            {
                Utility.PrintMessage("You have no transactions yet");
            }
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descripton", $"{AppScreen.currency}Amount");
                foreach (var transaction in filteredTransactionList)
                {
                    table.AddRow(transaction.TransactionId, transaction.TransactionDate, transaction.TransactionType, transaction.Description, transaction.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)");
            }
        }

        private void ProcessInternalTransfer(InternalTransfer internalTransfer)
        {
            if (internalTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be greater than 0. Please try again.", false);
                return;
            }
            //check senders account balance
            if (internalTransfer.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Insufficient funds. You do not have enough funds to transfer {Utility.FormatCurrency(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check minimumKpetAmount
            if ((selectedAccount.AccountBalance - internalTransfer.TransferAmount) < minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have a minimum of {Utility.FormatCurrency(minimumKeptAmount)} after a transaction", false);
                return;
            }

            //check recipient account is valid
            var recipientBankAccount = userAccounts.Where(x => x.AccountNumber == internalTransfer.RecipientBankAccoutNumber).FirstOrDefault();

            if (recipientBankAccount == null)
            {
                Utility.PrintMessage("Transfer failed. Recipient's bank account could not be found", false);
                return;
            }

            //check recipient's name

            if (recipientBankAccount.FullName.ToLower() != internalTransfer.RecipientBankAccountName.ToLower())
            {
                Utility.PrintMessage("Transfer failed. Recipient's name does not match listed name for this account.", false);
                return;
            }

            //add transaction to transaction record - sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, -internalTransfer.TransferAmount, $"Transferred to {recipientBankAccount.AccountNumber} ({recipientBankAccount.FullName})");
            //update sender's balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction to transaction record - recipient
            InsertTransaction(recipientBankAccount.Id, TransactionType.Transfer, internalTransfer.TransferAmount, $"Received from {selectedAccount.AccountNumber} ({selectedAccount.FullName})");
            //update recipient's balance
            recipientBankAccount.AccountBalance += internalTransfer.TransferAmount;
            //print success message to the console
            Utility.PrintMessage($"You have successfully transferred {Utility.FormatCurrency(internalTransfer.TransferAmount)} to {internalTransfer.RecipientBankAccountName}");
        }
    }
}