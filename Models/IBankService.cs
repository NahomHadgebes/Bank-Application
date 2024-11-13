using BankApplication.Models;
namespace BankApplication.Models

{
    public interface IBankService
    {
        void CreateAccount(string accountHolder, decimal initialBalance);
        void MakeTransaction(int accountNumber, decimal amount, string type);
        List<Account> GetAllAccounts();
        List<Transaction> GetTransactionHistory(int accountNumber);
        void LoadData();
        void SaveData();
    }
}


