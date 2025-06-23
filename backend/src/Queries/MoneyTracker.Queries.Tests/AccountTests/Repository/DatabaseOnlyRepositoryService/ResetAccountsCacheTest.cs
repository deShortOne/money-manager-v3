
using MoneyTracker.Authentication.DTOs;
using Moq;

namespace MoneyTracker.Queries.Tests.AccountTests.Repository.DatabaseOnlyRepositoryService;
public class ResetAccountCacheTest : DatabaseOnlyTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task FailsToResetData()
    {
        await Assert.ThrowsAsync<NotImplementedException>(async ()
            => await _accountRepositoryService.ResetAccountsCache(It.IsAny<AuthenticatedUser>()));
    }
}
