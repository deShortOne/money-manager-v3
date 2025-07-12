using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetAccountUserEntityByAccountNameTest : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetAccountUserEntityByAccountNameTest(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        _accountRepo = new AccountCommandRepository(_database);
    }

    public static TheoryData<int, string, int, bool, int> SeedData = new() {
        { 1, "Bank A", 1, true, 1 },
        { 2, "Bank A", 2, true, 1 },
        { 3, "Bank B", 1, true, 2 },
    };

    [Theory, MemberData(nameof(SeedData))]
    public async Task GetUserAccountEntity(int id, string accountName, int userId, bool expectedUserOwnsAccount, int accountId)
    {
        Assert.Equal(new AccountUserEntity(id, accountId, userId, expectedUserOwnsAccount),
            await _accountRepo.GetAccountUserEntity(accountName, userId, CancellationToken.None));
    }
}
