using FluentValidation;
using BankApplication.Models;

namespace BankApplication
{
    public class TransactionValidator : AbstractValidator<Transaction>
    {
        public TransactionValidator()
        {
            RuleFor(x => x.AccountNumber).GreaterThan(0).WithMessage("The account number must be greater than 0.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("The amount must be greater than 0.");
            RuleFor(x => x.Type).Must(BeAValidTransactionType).WithMessage("Invalid transaction type. Use 'deposit' or 'withdrawal.'");
        }
        private bool BeAValidTransactionType(string type)
        {
            return type == "deposit" || type == "withdrawal";
        }
    }
}


