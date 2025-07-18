using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Account;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.DatabaseOnlyRepositoryService;
public class GetAccountsTest : DatabaseOnlyTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task CallOffToDatabaseOnce()
    {
        var accounts = new List<AccountEntity>
        {
            new(8, "fds"),
            new(3, "iolk,"),
            new(1, "iukhm"),
        };

        _mockAccountDatabase.Setup(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None))
            .ReturnsAsync(accounts);

        await _accountRepositoryService.GetAccounts(_authedUser, CancellationToken.None);

        _mockAccountDatabase.Verify(x => x.GetAccountsOwnedByUser(_authedUser, CancellationToken.None), Times.Once);
        VerifyNoOtherCalls();
    }
}
