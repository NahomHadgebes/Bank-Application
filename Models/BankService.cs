using System.Text.Json;

namespace BankApplication.Models
{
    public class BankService : IBankService
    {
        private List<Account> accounts = new List<Account>();
        private List<Transaction> transactions = new List<Transaction>();

        public void CreateAccount(string accountHolder, decimal initialBalance)
        {
            if (accounts.Any(a => a.AccountHolder.Equals(accountHolder, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("An account with this holder already exists.");
            }

            int accountNumber = accounts.Count > 0 ? accounts.Max(a => a.AccountNumber) + 1 : 1;

            var newAccount = new Account
            {
                AccountNumber = accountNumber,
                AccountHolder = accountHolder,
                Balance = initialBalance
            };

            accounts.Add(newAccount);
            SaveData();
        }

        public void MakeTransaction(int accountNumber, decimal amount, string type)
        {
            var account = accounts.Find(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            if (type == "Deposit")
            {
                account.Balance += amount;
            }
            else if (type == "Withdrawal")
            {
                if (account.Balance >= amount)
                {
                    account.Balance -= amount;
                }
                else
                {
                    throw new Exception("Insufficient funds.");
                }
            }
            else
            {
                throw new Exception("Invalid transaction type.");
            }

            transactions.Add(new Transaction
            {
                AccountNumber = accountNumber,
                Amount = amount,
                Type = type.ToLower(),
                Date = DateTime.Now
            });

            SaveData();
        }

        public List<Account> GetAllAccounts()
        {
            return accounts;
        }
        public List<Transaction> GetTransactionHistory(int accountNumber)
        {
            return transactions.FindAll(t => t.AccountNumber == accountNumber);
        }
        public void SaveData()
        {
            const string fileName = "BankData.json";

            var data = new
            {
                Accounts = accounts,
                Transactions = transactions
            };

            var jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, jsonData);
        }
        public void LoadData()
        {
            const string fileName = "BankData.json";

            if (File.Exists(fileName))
            {
                var jsonData = File.ReadAllText(fileName);

                var data = JsonSerializer.Deserialize<DataBas>(jsonData);

                if (data != null)
                {
                    accounts = data.Accounts ?? new List<Account>();
                    transactions = data.Transactions ?? new List<Transaction>();
                }
            }
        }
        private int GenerateUniqueAccountNumber()
        {
            int newAccountNumber;
            do
            {
                newAccountNumber = accounts.Count > 0 ? accounts.Max(a => a.AccountNumber) + 1 : 1;
            } while (accounts.Any(a => a.AccountNumber == newAccountNumber));

            return newAccountNumber;
        }

    }
}






