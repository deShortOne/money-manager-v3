using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.TransactionTests.Repository;
public sealed class GetLastTransactionIdTest : IClassFixture<PostgresDbFixture>
{
    private RegisterCommandRepository _registerRepo;

    public GetLastTransactionIdTest(PostgresDbFixture postgresFixture)
    {
        var _database = new PostgresDatabase(postgresFixture.ConnectionString);
        _registerRepo = new RegisterCommandRepository(_database);
    }

    [Fact]
    public async Task GetLastTransactionId()
    {
        Assert.Equal(10, await _registerRepo.GetLastTransactionId(CancellationToken.None));
    }
}
