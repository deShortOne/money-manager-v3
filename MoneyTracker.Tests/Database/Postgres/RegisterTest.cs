
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;
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
            .WithName("aa")
            .WithReuse(true)
            .WithCleanUp(false)
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();

            Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true, true));

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

            var expected = new List<TransactionEntityDTO>()
            {
                new TransactionEntityDTO(
                    1,
                    "Company A",
                    1800,
                    new DateOnly(2024, 8, 28),
                    "Wages & Salary : Net Pay",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    6,
                    "Supermarket",
                    27,
                    new DateOnly(2024,8,15),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    7,
                    "Hobby item",
                    150,
                    new DateOnly(2024, 8, 9),
                    "Hobby",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    5,
                    "Supermarket",
                    23,
                    new DateOnly(2024, 8, 8),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    2,
                    "Phone company",
                    10,
                    new DateOnly(2024, 8, 1),
                    "Bills : Cell Phone",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    3,
                    "Landlord A",
                    500,
                    new DateOnly(2024, 8, 1),
                    "Bills : Rent",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    4,
                    "Supermarket",
                    25,
                    new DateOnly(2024, 8, 1),
                    "Groceries",
                    "bank b"
                ),
            };
            var actual = await register.GetAllTransactions(new AuthenticatedUser(1));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void FirstLoadCheckTablesThatDataAreThereForUser2()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);

            var expected = new List<TransactionEntityDTO>()
            {
                new TransactionEntityDTO(
                    10,
                    "Football kit",
                    100,
                    new DateOnly(2024, 8, 30),
                    "Hobby",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    9,
                    "Vet",
                    75,
                    new DateOnly(2024, 8, 29),
                    "Pet Care",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    8,
                    "Company A",
                    1500,
                    new DateOnly(2024, 8, 28),
                    "Wages & Salary : Net Pay",
                    "bank a"
                ),
            };
            var actual = await register.GetAllTransactions(new AuthenticatedUser(2));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void AddItemAfterFirstLoad()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);
            var transactionToAdd = new TransactionEntityDTO(
                11,
                "Super star",
                2300,
                new DateOnly(2024, 9, 1),
                "Hobby", // which is 5
                "bank a"
            );
            await register.AddTransaction(new NewTransactionDTO(
                transactionToAdd.Payee,
                transactionToAdd.Amount,
                transactionToAdd.DatePaid,
                5, // id correlate to hobby
                1
            ));

            var expected = new List<TransactionEntityDTO>()
            {
                transactionToAdd,
                new TransactionEntityDTO(
                    1,
                    "Company A",
                    1800,
                    new DateOnly(2024, 8, 28),
                    "Wages & Salary : Net Pay",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    6,
                    "Supermarket",
                    27,
                    new DateOnly(2024, 8, 15),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    7,
                    "Hobby item",
                    150,
                    new DateOnly(2024, 8, 9),
                    "Hobby",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    5,
                    "Supermarket",
                    23,
                    new DateOnly(2024, 8, 8),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    2,
                    "Phone company",
                    10,
                    new DateOnly(2024, 8, 1),
                    "Bills : Cell Phone",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    3,
                    "Landlord A",
                    500,
                    new DateOnly(2024, 8, 1),
                    "Bills : Rent",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    4,
                    "Supermarket",
                    25,
                    new DateOnly(2024, 8, 1),
                    "Groceries",
                    "bank b"
                ),
            };
            var actual = await register.GetAllTransactions(new AuthenticatedUser(1));
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

            var expected = new List<TransactionEntityDTO>()
            {
                new TransactionEntityDTO(
                    1,
                    "Company A",
                    1800,
                    new DateOnly(2024, 8, 28),
                    "Wages & Salary : Net Pay",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    6,
                    "Bar",
                    27,
                    new DateOnly(2024, 8, 15),
                    "Hobby",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    7,
                    "Hobby item",
                    150,
                    new DateOnly(2024, 8, 9),
                    "Hobby",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    5,
                    "Supermarket",
                    23,
                    new DateOnly(2024, 8, 8),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    2,
                    "Phone company",
                    10,
                    new DateOnly(2024, 8, 1),
                    "Bills : Cell Phone",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    3,
                    "Landlord A",
                    500,
                    new DateOnly(2024, 8, 1),
                    "Bills : Rent",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    4,
                    "Supermarket",
                    25,
                    new DateOnly(2024, 8, 1),
                    "Groceries",
                    "bank b"
                ),
            };
            var actual = await register.GetAllTransactions(new AuthenticatedUser(1));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void DeleteTransaction()
        {
            var db = new PostgresDatabase(_postgres.GetConnectionString());
            var register = new RegisterDatabase(db);

            await register.DeleteTransaction(new DeleteTransactionDTO(6));

            var expected = new List<TransactionEntityDTO>()
            {
                new TransactionEntityDTO(
                    1,
                    "Company A",
                    1800,
                    new DateOnly(2024, 8, 28),
                    "Wages & Salary : Net Pay",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    7,
                    "Hobby item",
                    150,
                    new DateOnly(2024, 8, 9),
                    "Hobby",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    5,
                    "Supermarket",
                    23,
                    new DateOnly(2024, 8, 8),
                    "Groceries",
                    "bank b"
                ),
                new TransactionEntityDTO(
                    2,
                    "Phone company",
                    10,
                    new DateOnly(2024, 8, 1),
                    "Bills : Cell Phone",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    3,
                    "Landlord A",
                    500,
                    new DateOnly(2024, 8, 1),
                    "Bills : Rent",
                    "bank a"
                ),
                new TransactionEntityDTO(
                    4,
                    "Supermarket",
                    25,
                    new DateOnly(2024, 8, 1),
                    "Groceries",
                    "bank b"
                ),
            };
            var actual = await register.GetAllTransactions(new AuthenticatedUser(1));
            Assert.Equal(expected, actual);
        }
    }
}
