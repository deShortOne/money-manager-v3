using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Bill;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.BillTests.Repository;
public sealed class GetBillsTest : IAsyncLifetime
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
    public async Task FirstLoadCheckTablesThatDataAreThereForUserId1()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
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
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var billDb = new BillDatabase(db);

        var actual = await billDb.GetAllBills(new AuthenticatedUser(2));

        var expected = new List<BillEntity>()
        {
            new(3, 12, "company a", 100, new DateOnly(2024, 8, 30), 30, "Monthly", 1, "Wages & Salary : Net Pay", 3, "bank a"),
        };

        Assert.Equal(expected, actual);
    }
}
