using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetLastAccountUserId : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetLastAccountUserId(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        Migration.CheckMigration(postgresDbFixture.ConnectionString, new MigrationOption(true));

        _accountRepo = new AccountCommandRepository(_database);
    }

    [Fact]
    public async Task GetLastIdWithDataInTables()
    {
        Assert.Equal(19, await _accountRepo.GetLastAccountUserId(CancellationToken.None));
    }
}
