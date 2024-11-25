using BankApplication.Models;
using Spectre.Console;
using System.Globalization;

namespace BankApplication
{
    public class LibraryMenu
    {
        public static void ShowMainMeny()
        {
            IBankService bankService = new BankService();
            bankService.LoadData();

            bool runApp = true;
            while (runApp)
            {
                Console.Clear();
                AnsiConsole.Markup("[bold cyan]Welcome to the Banking Application![/]");
                Console.WriteLine();

                var table = new Table();
                table.AddColumn("[yellow]Option[/]");
                table.AddColumn("[yellow]Description[/]");

                table.AddRow("Create Account", "Create a new bank account");
                table.AddRow("Make a transaction", "Deposit or withdraw money");
                table.AddRow("View all accounts", "View all current accounts");
                table.AddRow("View transaction history", "View history of transactions");
                table.AddRow("Exit", "Exit the application");

                AnsiConsole.Write(table);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please select an option")
                        .AddChoices("Create Account", "Make a transaction", "View all accounts", "View transaction history", "Exit"));

                switch (choice)
                {
                    case "Create Account":
                        SkapaKonto(bankService);
                        break;
                    case "Make a transaction":
                        GörTransaktion(bankService);
                        break;
                    case "View all accounts":
                        VisaAllaKonton(bankService);
                        break;
                    case "View transaction history":
                        VisaTransaktionsHistorik(bankService);
                        break;
                    case "Exit":
                        runApp = false;
                        bankService.SaveData();
                        AnsiConsole.Markup("[bold green]Thank you for using the banking application![/]");
                        break;
                    default:
                        AnsiConsole.Markup("[bold red]Invalid option. Please try again.[/]");
                        break;
                }
            }
        }
        static void SkapaKonto(IBankService bankService)
        {
            Console.Write("Enter the account holder: ");
            string accountHolder = HämtaGiltigtKontoinnehavare();

            Console.Write("Enter the starting amount: ");
            decimal balance = HämtaGiltigDecimalInput();

            bankService.CreateAccount(accountHolder, balance);
            Console.WriteLine("Account created successfully!");

            var accounts = bankService.GetAllAccounts();
            var createdAccount = accounts.Last();

            AnsiConsole.MarkupLine($"[bold green]Account Number: {createdAccount.AccountNumber}[/]");
            Console.WriteLine("Press any key to return to the main menu.");
            Console.ReadKey();
        }

        static void GörTransaktion(IBankService bankService)
        {
            var accounts = bankService.GetAllAccounts();
            var accountChoices = accounts.Select(a => $"{a.AccountNumber} - {a.AccountHolder} - Balance: {a.Balance} SEK").ToList();

            string selectedAccount = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an account for the transaction")
                    .AddChoices(accountChoices)
            );

            int selectedAccountNumber = Convert.ToInt32(selectedAccount.Split('-')[0].Trim());

            Console.Write("Enter amount: ");
            decimal amount = HämtaGiltigDecimalInput();

            string type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select transaction type")
                    .AddChoices("Deposit", "Withdrawal"));

            try
            {
                var transactionValidator = new TransactionValidator();
                var transaction = new Transaction
                {
                    AccountNumber = selectedAccountNumber,
                    Amount = amount,
                    Type = type.ToLower(),
                    Date = DateTime.Now
                };

                var validationResult = transactionValidator.Validate(transaction);

                if (validationResult.IsValid)
                {
                    AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("[green]Processing Transaction...[/]");
                            for (int i = 0; i < 10; i++)
                            {
                                task.Increment(10);
                                Thread.Sleep(100);
                            }
                        });

                    bankService.MakeTransaction(selectedAccountNumber, amount, type);

                    AnsiConsole.Markup("[bold green]Transaction completed successfully![/]\n");
                }
                else
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Console.WriteLine($"Error: {failure.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred:[/] {ex.Message}");
            }

            Console.ReadKey();
        }
        static void VisaAllaKonton(IBankService bankService)
        {
            var accounts = bankService.GetAllAccounts();
            if (accounts.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold yellow]No accounts found![/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("[yellow]Account Number[/]");
                table.AddColumn("[yellow]Account Holder[/]");
                table.AddColumn("[yellow]Balance[/]");

                foreach (var account in accounts)
                {
                    table.AddRow(account.AccountNumber.ToString(), account.AccountHolder, account.Balance.ToString("0.00") + " SEK");
                }

                AnsiConsole.Write(table);
            }

            Console.ReadKey();
        }
        static void VisaTransaktionsHistorik(IBankService bankService)
        {
            var accounts = bankService.GetAllAccounts();
            if (accounts.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold yellow]No accounts available to view transaction history![/]");
                Console.ReadKey();
                return;
            }

            var accountChoices = accounts.Select(a => $"{a.AccountNumber} - {a.AccountHolder} - Balance: {a.Balance} SEK").ToList();

            string selectedAccount = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an account to view transaction history")
                    .AddChoices(accountChoices)
            );

            int selectedAccountNumber = Convert.ToInt32(selectedAccount.Split('-')[0].Trim());

            var transactions = bankService.GetTransactionHistory(selectedAccountNumber);

            if (transactions.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold yellow]No transaction history for this account![/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("[yellow]Date[/]");
                table.AddColumn("[yellow]Type[/]");
                table.AddColumn("[yellow]Amount[/]");

                foreach (var transaction in transactions)
                {
                    table.AddRow(transaction.Date.ToString("g"), transaction.Type, $"{transaction.Amount} SEK");
                }

                AnsiConsole.Write(table);
            }

            Console.ReadKey();
        }

        static decimal HämtaGiltigDecimalInput()
        {
            decimal value;
            while (true)
            {
                string input = Console.ReadLine();
                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value) && value > 0)
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid positive decimal amount.");
                }
            }
        }
        static string HämtaGiltigtKontoinnehavare()
        {
            while (true)
            {
                string accountHolder = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(accountHolder))
                {
                    return accountHolder;
                }
                else
                {
                    Console.WriteLine("Invalid input. Account holder's name cannot be empty. Please try again:");
                }
            }
        }

    }
}



