namespace MoneyTracker.Queries.Tests.UserTests.Repository.DatabaseOnlyRepositoryService;
public class GetUserByUsernameTest : CacheAsideTestHelper
{
    string _username = "gdfsasfag";

    [Fact]
    public async Task WillCallOffToDatabase()
    {
        await _userRepositoryService.GetUserByUsername(_username);

        _mockUserDatabase.Verify(x => x.GetUserByUsername(_username));
        VerifyNoOtherCalls();
    }
}
