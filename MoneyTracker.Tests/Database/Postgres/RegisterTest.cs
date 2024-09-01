using DatabaseMigration;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Transaction;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Database.Postgres
{
    public sealed class RegisterTest : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
            .WithDockerEndpoint("tcp://localhost:2375")
#endif
            .WithImage("postgres:16")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            Migration.CheckMigration(_postgres.GetConnectionString());

            return;
        }

        public Task DisposeAsync()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [Fact]
        public async void FirstLoadCheckTablesThatDataAreThere()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var register = new Register(db);

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO() {
                    Id = 1,
                    Payee = "Company A",
                    Amount = 1800,
                    DatePaid = DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    Category = "Wages & Salary : Net Pay",
                },
                new TransactionDTO() {
                    Id = 6,
                    Payee = "Supermarket",
                    Amount = 27,
                    DatePaid = DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 7,
                    Payee = "Hobby item",
                    Amount = 150,
                    DatePaid = DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 5,
                    Payee = "Supermarket",
                    Amount = 23,
                    DatePaid = DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 2,
                    Payee = "Phone company",
                    Amount = 10,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Cell Phone",
                },
                new TransactionDTO() {
                    Id = 3,
                    Payee = "Landlord A",
                    Amount = 500,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Rent",
                },
                new TransactionDTO() {
                    Id = 4,
                    Payee = "Supermarket",
                    Amount = 25,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
            };
            var actual = await register.GetAllTransactions();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void AddItemAfterFirstLoad()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var register = new Register(db);
            var transactionToAdd = new TransactionDTO()
            {
                Id = 8,
                Payee = "Super star",
                Amount = 2300,
                DatePaid = DateTime.Parse("2024-09-01T00:00:00Z").ToUniversalTime(),
                Category = "Hobby", // which is 5
            };
            await register.AddNewTransaction(new NewTransactionDTO()
            {
                Amount = transactionToAdd.Amount,
                Category = 5, // id correlate to hobby
                DatePaid = transactionToAdd.DatePaid,
                Payee = transactionToAdd.Payee,
            });

            var expected = new List<TransactionDTO>()
            {
                transactionToAdd,
                new TransactionDTO() {
                    Id = 1,
                    Payee = "Company A",
                    Amount = 1800,
                    DatePaid = DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    Category = "Wages & Salary : Net Pay",
                },
                new TransactionDTO() {
                    Id = 6,
                    Payee = "Supermarket",
                    Amount = 27,
                    DatePaid = DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 7,
                    Payee = "Hobby item",
                    Amount = 150,
                    DatePaid = DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 5,
                    Payee = "Supermarket",
                    Amount = 23,
                    DatePaid = DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 2,
                    Payee = "Phone company",
                    Amount = 10,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Cell Phone",
                },
                new TransactionDTO() {
                    Id = 3,
                    Payee = "Landlord A",
                    Amount = 500,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Rent",
                },
                new TransactionDTO() {
                    Id = 4,
                    Payee = "Supermarket",
                    Amount = 25,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
            };
            var actual = await register.GetAllTransactions();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void EditATransaction()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var register = new Register(db);

            await register.EditTransaction(new EditTransactionDTO()
            {
                Id = 6,
                Payee = "Bar",
                Category = 5, // "Hobby"
            });

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO() {
                    Id = 1,
                    Payee = "Company A",
                    Amount = 1800,
                    DatePaid = DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    Category = "Wages & Salary : Net Pay",
                },
                new TransactionDTO() {
                    Id = 6,
                    Payee = "Bar",
                    Amount = 27,
                    DatePaid = DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 7,
                    Payee = "Hobby item",
                    Amount = 150,
                    DatePaid = DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 5,
                    Payee = "Supermarket",
                    Amount = 23,
                    DatePaid = DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 2,
                    Payee = "Phone company",
                    Amount = 10,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Cell Phone",
                },
                new TransactionDTO() {
                    Id = 3,
                    Payee = "Landlord A",
                    Amount = 500,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Rent",
                },
                new TransactionDTO() {
                    Id = 4,
                    Payee = "Supermarket",
                    Amount = 25,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
            };
            var actual = await register.GetAllTransactions();
            TestHelper.CompareLists(expected, actual);
        }

        [Fact]
        public async void DeleteTransaction()
        {
            var db = new Helper(_postgres.GetConnectionString());
            var register = new Register(db);

            await register.DeleteTransaction(new DeleteTransactionDTO()
            {
                Id = 6,
            });

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO() {
                    Id = 1,
                    Payee = "Company A",
                    Amount = 1800,
                    DatePaid = DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    Category = "Wages & Salary : Net Pay",
                },
                new TransactionDTO() {
                    Id = 7,
                    Payee = "Hobby item",
                    Amount = 150,
                    DatePaid = DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 5,
                    Payee = "Supermarket",
                    Amount = 23,
                    DatePaid = DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 2,
                    Payee = "Phone company",
                    Amount = 10,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Cell Phone",
                },
                new TransactionDTO() {
                    Id = 3,
                    Payee = "Landlord A",
                    Amount = 500,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Bills : Rent",
                },
                new TransactionDTO() {
                    Id = 4,
                    Payee = "Supermarket",
                    Amount = 25,
                    DatePaid = DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
            };
            var actual = await register.GetAllTransactions();
            TestHelper.CompareLists(expected, actual);
        }
    }
}
