using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetLastAccountIdTest : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetLastAccountIdTest(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        Migration.CheckMigration(postgresDbFixture.ConnectionString, new MigrationOption(true));

        _accountRepo = new AccountCommandRepository(_database);
    }

    [Fact]
    public async Task GetLastIdWithDataInTables()
    {
        Assert.Equal(10, await _accountRepo.GetLastAccountId(CancellationToken.None));
    }
}
