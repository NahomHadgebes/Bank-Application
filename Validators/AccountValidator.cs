using BankApplication.Models;
using FluentValidation;

namespace BankApplication.Validators
{
    public class AccountValidator : AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(account => account.AccountHolder)
                .NotEmpty().WithMessage("Kontoinnehavarens namn får inte vara tomt.")
                .Length(2, 50).WithMessage("Kontoinnehavarens namn måste vara mellan 2 och 50 tecken.");

            RuleFor(account => account.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Saldo kan inte vara negativt.");
        }
    }
}

