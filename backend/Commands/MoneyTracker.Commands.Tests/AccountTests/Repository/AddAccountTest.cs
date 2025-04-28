using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Account;

namespace MoneyTracker.Commands.Tests.AccountTests.Repository;
public class AddAccountTest : AccountRespositoryTestHelper
{
    [Fact]
    public async Task AddAccountIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var allAccountsInitiallyCount = (await GetAllAccountEntity()).Count;

        var accountToAdd = new AccountEntity(61252, "gasdlkfhjsadfl");
        await _accountRepo.AddAccount(accountToAdd);

        var results = await GetAllAccountEntity();
        Assert.Equal(accountToAdd, results[allAccountsInitiallyCount]);
    }
}
