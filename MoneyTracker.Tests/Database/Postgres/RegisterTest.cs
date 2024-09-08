
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
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

            Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

            return;
        }

        public Task DisposeAsync()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [Fact]
        public async void FirstLoadCheckTablesThatDataAreThere()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO(
                    1,
                    "Company A",
                    1800,
                    DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    "Wages & Salary : Net Pay"
                ),
                new TransactionDTO(
                    6,
                    "Supermarket",
                    27,
                    DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    7,
                    "Hobby item",
                    150,
                    DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    "Hobby"
                ),
                new TransactionDTO(
                    5,
                    "Supermarket",
                    23,
                    DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    2,
                    "Phone company",
                    10,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Cell Phone"
                ),
                new TransactionDTO(
                    3,
                    "Landlord A",
                    500,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Rent"
                ),
                new TransactionDTO(
                    4,
                    "Supermarket",
                    25,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
            };
            var actual = await register.GetAllTransactions();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void AddItemAfterFirstLoad()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);
            var transactionToAdd = new TransactionDTO(
                8,
                "Super star",
                2300,
                DateTime.Parse("2024-09-01T00:00:00Z").ToUniversalTime(),
                "Hobby" // which is 5
            );
            await register.AddNewTransaction(new NewTransactionDTO(
                transactionToAdd.Payee,
                transactionToAdd.Amount,
                transactionToAdd.DatePaid,
                5 // id correlate to hobby
            ));

            var expected = new List<TransactionDTO>()
            {
                transactionToAdd,
                new TransactionDTO(
                    1,
                    "Company A",
                    1800,
                    DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    "Wages & Salary : Net Pay"
                ),
                new TransactionDTO(
                    6,
                    "Supermarket",
                    27,
                    DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    7,
                    "Hobby item",
                    150,
                    DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    "Hobby"
                ),
                new TransactionDTO(
                    5,
                    "Supermarket",
                    23,
                    DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    2,
                    "Phone company",
                    10,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Cell Phone"
                ),
                new TransactionDTO(
                    3,
                    "Landlord A",
                    500,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Rent"
                ),
                new TransactionDTO(
                    4,
                    "Supermarket",
                    25,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
            };
            var actual = await register.GetAllTransactions();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void EditATransaction()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);

            await register.EditTransaction(new EditTransactionDTO(
                6,
                "Bar",
                category: 5 // "Hobby"
            ));

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO(
                    1,
                    "Company A",
                    1800,
                    DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    "Wages & Salary : Net Pay"
                ),
                new TransactionDTO(
                    6,
                    "Bar",
                    27,
                    DateTime.Parse("2024-08-15T00:00:00Z").ToUniversalTime(),
                    "Hobby"
                ),
                new TransactionDTO(
                    7,
                    "Hobby item",
                    150,
                    DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    "Hobby"
                ),
                new TransactionDTO(
                    5,
                    "Supermarket",
                    23,
                    DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    2,
                    "Phone company",
                    10,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Cell Phone"
                ),
                new TransactionDTO(
                    3,
                    "Landlord A",
                    500,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Rent"
                ),
                new TransactionDTO(
                    4,
                    "Supermarket",
                    25,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
            };
            var actual = await register.GetAllTransactions();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void DeleteTransaction()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);

            await register.DeleteTransaction(new DeleteTransactionDTO()
            {
                Id = 6,
            });

            var expected = new List<TransactionDTO>()
            {
                new TransactionDTO(
                    1,
                    "Company A",
                    1800,
                    DateTime.Parse("2024-08-28T00:00:00Z").ToUniversalTime(),
                    "Wages & Salary : Net Pay"
                ),
                new TransactionDTO(
                    7,
                    "Hobby item",
                    150,
                    DateTime.Parse("2024-08-09T00:00:00Z").ToUniversalTime(),
                    "Hobby"
                ),
                new TransactionDTO(
                    5,
                    "Supermarket",
                    23,
                    DateTime.Parse("2024-08-08T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
                new TransactionDTO(
                    2,
                    "Phone company",
                    10,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Cell Phone"
                ),
                new TransactionDTO(
                    3,
                    "Landlord A",
                    500,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Bills : Rent"
                ),
                new TransactionDTO(
                    4,
                    "Supermarket",
                    25,
                    DateTime.Parse("2024-08-01T00:00:00Z").ToUniversalTime(),
                    "Groceries"
                ),
            };
            var actual = await register.GetAllTransactions();
            Assert.Equal(expected, actual);
        }
    }
}
