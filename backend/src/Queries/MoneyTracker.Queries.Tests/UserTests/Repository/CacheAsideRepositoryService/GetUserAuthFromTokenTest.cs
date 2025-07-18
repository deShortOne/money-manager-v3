namespace MoneyTracker.Queries.Tests.UserTests.Repository.CacheAsideRepositoryService;
public class GetUserAuthFromTokenTest : CacheAsideTestHelper
{
    string _token = "gdfsasfag";

    [Fact]
    public async Task WillCallOffToDatabase()
    {
        await _userRepositoryService.GetUserAuthFromToken(_token, CancellationToken.None);

        _mockUserDatabase.Verify(x => x.GetUserAuthFromToken(_token, CancellationToken.None));
        VerifyNoOtherCalls();
    }
}
