using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.BudgetTests.Repository;
public sealed class GetBudgetTest : IAsyncLifetime
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
        var budgetDb = new BudgetRepository(db);

        var actual = await budgetDb.GetBudget(new AuthenticatedUser(1));

        var expected = new List<BudgetGroupEntity>()
        {
            new("Income", 1800, 1800, 0, [new("Wages & Salary : Net Pay", 1800, 1800, 0)]),
            new("Committed Expenses", 610, 585, 25, [
                new("Bills : Cell Phone", 10, 10, 0),
                new("Bills : Rent", 500, 500, 0),
                new("Groceries", 100, 75, 25),
            ]),
            new("Fun", 0, 0, 0, []),
            new("Irregular Expenses", 50, 150, -100, [new("Hobby", 50, 150, -100)]),
            new("Savings & Debt", 0, 0, 0, []),
            new("Retirement", 0, 0, 0, []),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThereForUserId2()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var budgetDb = new BudgetRepository(db);

        var actual = await budgetDb.GetBudget(new AuthenticatedUser(2));

        var expected = new List<BudgetGroupEntity>()
        {
            new("Income", 0, 0, 0, []),
            new("Committed Expenses", 0, 0, 0, []),
            new("Fun", 0, 0, 0, []),
            new("Irregular Expenses", 0, 0, 0, []),
            new("Savings & Debt", 0, 0, 0, []),
            new("Retirement", 0, 0, 0, []),
        };

        Assert.Equal(expected, actual);
    }
}
