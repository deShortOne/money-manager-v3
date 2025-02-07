using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.CacheAsideRepositoryService;
public class ResetUsersCacheTest : CacheAsideTestHelper
{
    AuthenticatedUser _authedUser = new(36);

    [Fact]
    public async Task RefetchDataFromDatabaseAndWriteIntoCache()
    {
        await _userRepositoryService.ResetUsersCache();

        VerifyNoOtherCalls();
    }
}
