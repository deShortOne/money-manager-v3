using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.BudgetTests.Repository.PostgresDb;
public sealed class GetBudgetTest : IClassFixture<PostgresDbFixture>
{
    private readonly PostgresDbFixture _postgresFixture;

    public GetBudgetTest(PostgresDbFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId1()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var budgetDb = new BudgetDatabase(db);

        var actual = await budgetDb.GetBudget(new AuthenticatedUser(1), CancellationToken.None);

        var expected = new List<BudgetGroupEntity>()
        {
            new(1, "Income", 1800, 1800, 0, [new(1, "Wages & Salary : Net Pay", 1800, 1800, 0)]),
            new(2, "Committed Expenses", 610, 585, 25, [
                new(2, "Bills : Cell Phone", 10, 10, 0),
                new(3, "Bills : Rent", 500, 500, 0),
                new(4, "Groceries", 100, 75, 25),
            ]),
            new(3, "Fun", 0, 0, 0, []),
            new(4, "Irregular Expenses", 50, 150, -100, [new(5, "Hobby", 50, 150, -100)]),
            new(5, "Savings & Debt", 0, 0, 0, []),
            new(6, "Retirement", 0, 0, 0, []),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId2()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var budgetDb = new BudgetDatabase(db);

        var actual = await budgetDb.GetBudget(new AuthenticatedUser(2), CancellationToken.None);

        var expected = new List<BudgetGroupEntity>()
        {
            new(1, "Income", 0, 0, 0, []),
            new(2, "Committed Expenses", 0, 0, 0, []),
            new(3, "Fun", 0, 0, 0, []),
            new(4, "Irregular Expenses", 0, 0, 0, []),
            new(5, "Savings & Debt", 0, 0, 0, []),
            new(6, "Retirement", 0, 0, 0, []),
        };

        Assert.Equal(expected, actual);
    }
}
