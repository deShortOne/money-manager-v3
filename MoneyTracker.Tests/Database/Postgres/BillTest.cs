
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Database.Postgres.TestModels;
using Newtonsoft.Json;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.Database.Postgres;
public class BillTest : IAsyncLifetime
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
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var bill = new BillDatabase(db);

        var expected = new List<BillDTO>()
        {
            new BillDTO(2, "company a", 100, DateOnly.Parse("2024-08-30"), "monthly", "Wages & Salary : Net Pay"),
            new BillDTO(1, "supermarket a", 23, DateOnly.Parse("2024-09-03"), "weekly", "Groceries"),
        };

        var actual = await bill.GetBill();

        Assert.Equal(expected, actual);
    }
}
