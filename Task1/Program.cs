using System;
using System.Collections.Generic;

namespace FinanceManagementDemo
{
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    public sealed class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing bank transfer: {transaction.Id}, Amount: {transaction.Amount}, Date: {transaction.Date}, Category: {transaction.Category}");
        }
    }

    public sealed class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing mobile money transfer: {transaction.Id}, Amount: {transaction.Amount}, Date: {transaction.Date}, Category: {transaction.Category}");
        }
    }

    public sealed class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing crypto wallet transaction: {transaction.Id}, Amount: {transaction.Amount}, Date: {transaction.Date}, Category: {transaction.Category}");
        }
    }

    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }
        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number must be provided", nameof(accountNumber));
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative");

            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0)
                throw new ArgumentOutOfRangeException(nameof(transaction.Amount), "Transaction amount must be non-negative");

            Balance -= transaction.Amount;
            Console.WriteLine($"[Account {AccountNumber}] Applied {transaction.Category}: -{transaction.Amount:C}. New balance: {Balance:C}");
        }
    }

    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0)
            {
                Console.WriteLine("Transaction amount must be non-negative.");
                return;
            }

            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[Savings {AccountNumber}] {transaction.Category} of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
        }
    }

    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            var savings = new SavingsAccount(accountNumber: "SA-001", initialBalance: 1000m);
            Console.WriteLine($"Opened SavingsAccount {savings.AccountNumber} with balance {savings.Balance:C}\n");

            var t1 = new Transaction(Id: 1, Date: DateTime.Now, Amount: 120.50m, Category: "Groceries");
            var t2 = new Transaction(Id: 2, Date: DateTime.Now, Amount: 300.00m, Category: "Utilities");
            var t3 = new Transaction(Id: 3, Date: DateTime.Now, Amount: 200.00m, Category: "Entertainment");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            p2.Process(t2);
            p3.Process(t3);
            Console.WriteLine();

            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("Transaction log:");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($" - #{tx.Id}: {tx.Category} | {tx.Date:g} | {tx.Amount:C}");
            }

            Console.WriteLine($"\nFinal Balance: {savings.Balance:C}");

        }
    }
    
    public static class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}