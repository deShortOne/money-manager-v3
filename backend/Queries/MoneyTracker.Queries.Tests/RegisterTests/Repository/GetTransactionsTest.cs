using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.RegisterTests.Repository;
public sealed class GetTransactionsTest : IAsyncLifetime
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
    public async void FirstLoadCheckTablesThatDataAreThereForUserId1()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterRepository(db);

        var actual = await registerDb.GetAllTransactions(new AuthenticatedUser(1));

        var expected = new List<TransactionEntity>()
        {
            new(1, "Company A", 1800, new DateOnly(2024, 8, 28), "Wages & Salary : Net Pay", "bank a"),
            new(6, "Supermarket", 27, new DateOnly(2024, 8, 15), "Groceries", "bank b"),
            new(7, "Hobby item", 150, new DateOnly(2024, 8, 9), "Hobby", "bank a"),
            new(5, "Supermarket", 23, new DateOnly(2024, 8, 8), "Groceries", "bank b"),
            new(2, "Phone company", 10, new DateOnly(2024, 8, 1), "Bills : Cell Phone", "bank a"),
            new(3, "Landlord A", 500, new DateOnly(2024, 8, 1), "Bills : Rent", "bank a"),
            new(4, "Supermarket", 25, new DateOnly(2024, 8, 1), "Groceries", "bank b"),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThereForUserId2()
    {
       var db = new PostgresDatabase(_postgres.GetConnectionString());
        var registerDb = new RegisterRepository(db);

        var actual = await registerDb.GetAllTransactions(new AuthenticatedUser(2));

        var expected = new List<TransactionEntity>()
        {
            new(10, "Football kit", 100, new DateOnly(2024, 8, 30), "Hobby", "bank a"),
            new(9, "Vet", 75, new DateOnly(2024, 8, 29), "Pet Care", "bank a"),
            new(8, "Company A", 1500, new DateOnly(2024, 8, 28), "Wages & Salary : Net Pay", "bank a"),
        };

        Assert.Equal(expected, actual);
    }
}
