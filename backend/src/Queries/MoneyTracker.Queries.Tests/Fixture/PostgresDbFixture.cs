using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Queries.Tests.Fixture;
public class PostgresDbFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(ConnectionString, new MigrationOption(true));
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
