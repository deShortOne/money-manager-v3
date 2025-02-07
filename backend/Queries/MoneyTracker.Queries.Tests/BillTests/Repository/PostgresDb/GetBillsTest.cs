using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.BillTests.Repository.PostgresDb;
public sealed class GetBillsTest : IClassFixture<PostgresDbFixture>
{
    private readonly PostgresDbFixture _postgresFixture;

    public GetBillsTest(PostgresDbFixture postgresFixture)
    {
        _postgresFixture = postgresFixture;
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId1()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var billDb = new BillDatabase(db);

        var actual = await billDb.GetAllBills(new AuthenticatedUser(1));

        var expected = new List<BillEntity>()
        {
            new(2, 12, "company a", 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, "Wages & Salary : Net Pay", 2, "bank b"),
            new(1, 11, "supermarket a", 23, new DateOnly(2024, 9, 3), 3, "Weekly", 4, "Groceries", 1, "bank a"),
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId2()
    {
        var db = new PostgresDatabase(_postgresFixture.ConnectionString);
        var billDb = new BillDatabase(db);

        var actual = await billDb.GetAllBills(new AuthenticatedUser(2));

        var expected = new List<BillEntity>()
        {
            new(3, 12, "company a", 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, "Wages & Salary : Net Pay", 3, "bank a"),
        };

        Assert.Equal(expected, actual);
    }
}
