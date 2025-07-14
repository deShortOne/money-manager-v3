using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class GetAccountUserEntityTest : IClassFixture<PostgresDbFixture>
{
    public IAccountCommandRepository _accountRepo;

    public GetAccountUserEntityTest(PostgresDbFixture postgresDbFixture)
    {
        var _database = new PostgresDatabase(postgresDbFixture.ConnectionString);
        _accountRepo = new AccountCommandRepository(_database);
    }

    public static TheoryData<int, int, int, bool> SeedData = new() {
        { 1, 1, 1, true },
        { 2, 1, 2, true },
        { 3, 2, 1, true },
    };

    [Theory, MemberData(nameof(SeedData))]
    public async Task GetUserAccountEntity(int id, int accountId, int userId, bool expectedUserOwnsAccount)
    {
        Assert.Equal(new AccountUserEntity(id, accountId, userId, expectedUserOwnsAccount),
            await _accountRepo.GetAccountUserEntity(accountId, userId, CancellationToken.None));
    }
}
