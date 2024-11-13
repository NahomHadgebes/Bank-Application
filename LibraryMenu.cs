using BankApplication.Models;
using Spectre.Console;

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

                table.AddRow("1", "Create Account");
                table.AddRow("2", "Make a transaction");
                table.AddRow("3", "View all accounts");
                table.AddRow("4", "View transaction history");
                table.AddRow("5", "Exit");

                AnsiConsole.Write(table);

                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Please select an option")
                        .AddChoices("1", "2", "3", "4", "5"));

                switch (choice)
                {
                    case "1":
                        SkapaKonto(bankService);
                        break;
                    case "2":
                        GörTransaktion(bankService);
                        break;
                    case "3":
                        VisaAllaKonton(bankService);
                        break;
                    case "4":
                        VisaTransaktionsHistorik(bankService);
                        break;
                    case "5":
                        runApp = false;
                        bankService.SaveData();
                        AnsiConsole.Markup("[bold green]Thank you for using the banking application![/]");
                        break;
                    default:
                        AnsiConsole.Markup("[bold red]Invalid option. Please try again.[/]"); ;
                        break;
                }
            }
        }
        static void SkapaKonto(IBankService bankService)
        {
            Console.Write("Enter the account holder: ");
            string accountHolder = Console.ReadLine();
            Console.Write("Enter the starting amount: ");
            decimal balance = Convert.ToDecimal(Console.ReadLine());

            bankService.CreateAccount(accountHolder, balance);
            Console.WriteLine("Account created successfully!");
            Console.WriteLine("Press any key to return to main menu ");
            Console.ReadKey();
        }
        static void GörTransaktion(IBankService bankService)
        {
            Console.Write("Enter the account number: ");
            int accountNumber = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter amount: ");
            decimal amount = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Enter transaction type (deposit/withdrawal): ");
            string type = Console.ReadLine();

            try
            {
                var transactionValidator = new TransactionValidator();
                var transaction = new Transaction
                {
                    AccountNumber = accountNumber,
                    Amount = amount,
                    Type = type,
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

                    bankService.MakeTransaction(accountNumber, amount, type);
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
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.ReadKey();
        }
        static void VisaAllaKonton(IBankService bankService)
        {
            var accounts = bankService.GetAllAccounts();

            var table = new Table();
            table.AddColumn("[yellow]Account Number[/]");
            table.AddColumn("[yellow]Account Holder[/]");
            table.AddColumn("[yellow]Balance[/]");

            foreach (var account in accounts)
            {
                table.AddRow(account.AccountNumber.ToString(), account.AccountHolder, account.Balance.ToString("0.00") + " SEK");
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }
        static void VisaTransaktionsHistorik(IBankService bankService)
        {
            Console.Write("Enter the account number: ");
            int accountNumber = Convert.ToInt32(Console.ReadLine());

            var transactions = bankService.GetTransactionHistory(accountNumber);
            if (transactions.Count > 0)
            {
                foreach (var transaction in transactions)
                {
                    Console.WriteLine($"{transaction.Date} - {transaction.Type} - {transaction.Amount} SEK");
                }
            }
            else
            {
                Console.WriteLine("No transaction history exists for this account.");
            }

            Console.ReadKey();
        }
    }
}

