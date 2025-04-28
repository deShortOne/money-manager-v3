
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.CacheAsideRepositoryService;
public class ResetAccountCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        var accounts = new List<AccountEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockAccountDatabase.Setup(x => x.GetAccountsOwnedByUser(_authedUser))
            .ReturnsAsync(accounts);

        await _accountRepositoryService.ResetAccountsCache(_authedUser);

        _mockAccountDatabase.Verify(x => x.GetAccountsOwnedByUser(_authedUser));
        _mockAccountCache.Verify(x => x.SaveAccounts(_authedUser, accounts));
        VerifyNoOtherCalls();
    }
}
