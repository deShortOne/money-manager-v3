using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.DatabaseOnlyRepositoryService;
public class ResetUsersCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await Assert.ThrowsAsync<NotImplementedException>(() => _userRepositoryService.ResetUsersCache());

        VerifyNoOtherCalls();
    }
}
