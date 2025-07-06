
using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Queries.Tests.RegisterTests.Repository.PostgresDb.GivenARequestToGetReceiptProcessingInfo;
public abstract class ReceiptProcessingInfoHelper : IAsyncLifetime
{

    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
         .WithDockerEndpoint("tcp://localhost:2375")
#endif
         .WithImage("postgres:16")
         .WithCleanUp(true)
         .Build();

    protected PostgresDatabase _database;
    public RegisterDatabase _registerDatabase;

    public virtual async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        _database = new PostgresDatabase(_postgres.GetConnectionString());
        _registerDatabase = new RegisterDatabase(_database);
    }

    public async Task DisposeAsync() => await _postgres.DisposeAsync();
}
