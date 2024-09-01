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

        public Task InitializeAsync()
        {
            return _postgres.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _postgres.DisposeAsync().AsTask();
        }

        [Fact]
        public async void Test1()
        {
            var register = new Register();
            Assert.Equal(new List<TransactionDTO>(), await register.GetAllTransactions());
        }
    }
}