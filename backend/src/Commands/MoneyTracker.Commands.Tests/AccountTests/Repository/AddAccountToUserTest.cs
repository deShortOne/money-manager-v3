using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Account;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class AddAccountToUserTest : AccountRespositoryTestHelper
{
    [Fact]
    public async Task AddAccountToUserIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var allAccountsToUserInitialCount = (await GetAllAccountUserEntity()).Count;

        var billToAdd = new AccountUserEntity(4536, 2, 2, true);
        await _accountRepo.AddAccountToUser(billToAdd, CancellationToken.None);

        var results = await GetAllAccountUserEntity();
        Assert.Equal(billToAdd, results[allAccountsToUserInitialCount]);
    }
}
