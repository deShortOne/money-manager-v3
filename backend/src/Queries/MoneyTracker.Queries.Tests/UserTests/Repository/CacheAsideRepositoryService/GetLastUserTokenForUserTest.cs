using MoneyTracker.Authentication.Entities;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.CacheAsideRepositoryService;
public class GetLastUserTokenForUserTest : CacheAsideTestHelper
{
    UserEntity _userEntity = new(36, "dsfg", "dfhg");

    [Fact]
    public async Task WillCallOffToDatabase()
    {
        await _userRepositoryService.GetLastUserTokenForUser(_userEntity, CancellationToken.None);

        _mockUserDatabase.Verify(x => x.GetLastUserTokenForUser(_userEntity, CancellationToken.None));
        VerifyNoOtherCalls();
    }
}
