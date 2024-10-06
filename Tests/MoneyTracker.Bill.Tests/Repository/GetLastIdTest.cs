
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Data;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Bill.Tests.Repository;
public sealed class GetLastIdTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

#pragma warning disable CS8618 // disable nullable
    private IBillDatabase _billRepo;
#pragma warning restore CS8618

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _billRepo = new BillDatabase(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void GetLastWithNoDataInTables()
    {
        Assert.Equal(0, await _billRepo.GetLastId());
    }

    [Fact]
    public async void GetLastWithDataInTables()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        Assert.Equal(3, await _billRepo.GetLastId());
    }
}
