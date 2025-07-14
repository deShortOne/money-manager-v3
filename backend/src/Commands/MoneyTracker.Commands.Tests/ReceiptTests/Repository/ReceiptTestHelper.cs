

using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.ReceiptTests.Repository;
public abstract class ReceiptTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();
    protected PostgresDatabase _database;
    public ReceiptCommandRepository _receiptRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _receiptRepo = new ReceiptCommandRepository(_database);
    }

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();
}
