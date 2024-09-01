using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Transaction;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests
{
    public sealed class UnitTest1 : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
            .WithDockerEndpoint("tcp://localhost:2375")
            .WithImage("postgres:16")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await _postgres.StartAsync();
            Program.Main([_postgres.GetConnectionString()]);
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

            var registerRes = new List<TransactionDTO>()
            {
                new TransactionDTO() {
                    Id = 1,
                    Payee = "Company A",
                    Amount = 1800,
                    DatePaid = DateTime.Parse("2024-08-27T23:00:00Z").ToUniversalTime(),
                    Category = "Wages & Salary : Net Pay",
                },
                new TransactionDTO() {
                    Id = 6,
                    Payee = "Supermarket",
                    Amount = 27,
                    DatePaid = DateTime.Parse("2024-08-14T23:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 7,
                    Payee = "Hobby item",
                    Amount = 150,
                    DatePaid = DateTime.Parse("2024-08-08T23:00:00Z").ToUniversalTime(),
                    Category = "Hobby",
                },
                new TransactionDTO() {
                    Id = 5,
                    Payee = "Supermarket",
                    Amount = 23,
                    DatePaid = DateTime.Parse("2024-08-07T23:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 4,
                    Payee = "Supermarket",
                    Amount = 25,
                    DatePaid = DateTime.Parse("2024-07-31T23:00:00Z").ToUniversalTime(),
                    Category = "Groceries",
                },
                new TransactionDTO() {
                    Id = 2,
                    Payee = "Phone company",
                    Amount = 10,
                    DatePaid = DateTime.Parse("2024-07-31T23:00:00Z").ToUniversalTime(),
                    Category = "Bills : Cell Phone",
                },
                new TransactionDTO() {
                    Id = 3,
                    Payee = "Landlord A",
                    Amount = 500,
                    DatePaid = DateTime.Parse("2024-07-31T23:00:00Z").ToUniversalTime(),
                    Category = "Bills : Rent",
                }
            };

            // wtf??? can someone explain to me why these aren't equal
            // the data above was pulled from the api.... -_-
            Assert.Equal(registerRes, await register.GetAllTransactions());
        }
    }
}