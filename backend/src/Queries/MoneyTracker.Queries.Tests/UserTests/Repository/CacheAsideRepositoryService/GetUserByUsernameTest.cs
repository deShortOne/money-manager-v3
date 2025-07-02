namespace MoneyTracker.Queries.Tests.UserTests.Repository.CacheAsideRepositoryService;
public class GetUserByUsernameTest : CacheAsideTestHelper
{
    string _username = "gdfsasfag";

    [Fact]
    public async Task WillCallOffToDatabase()
    {
        await _userRepositoryService.GetUserByUsername(_username, CancellationToken.None);

        _mockUserDatabase.Verify(x => x.GetUserByUsername(_username, CancellationToken.None));
        VerifyNoOtherCalls();
    }
}
