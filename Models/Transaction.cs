namespace BankApplication.Models
{
    public class Transaction
    {
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}


