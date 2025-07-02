using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Account;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.CacheAsideRepositoryService;
public class GetAccountsTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task DataInCacheWontCallOffToDatabase()
    {
        _mockAccountCache.Setup(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None))
            .ReturnsAsync(new List<AccountEntity>());

        await _accountRepositoryService.GetAccounts(_authedUser, CancellationToken.None);

        _mockAccountCache.Verify(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None));
        VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DataNotInCacheWillCallOffToDatabaseAndSaveInCache()
    {
        var accounts = new List<AccountEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockAccountCache.Setup(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None))
            .ReturnsAsync(Error.NotFound("", ""));
        _mockAccountDatabase.Setup(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None))
            .ReturnsAsync(accounts);

        await _accountRepositoryService.GetAccounts(_authedUser, CancellationToken.None);

        _mockAccountCache.Verify(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None));
        _mockAccountDatabase.Verify(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None));
        _mockAccountCache.Verify(x => x.SaveAccounts(_authedUser, accounts));
        VerifyNoOtherCalls();
    }
}
