namespace MoneyTracker.Queries.Tests.UserTests.Repository.PostgresDb;
public sealed class GetUserAuthFromTokenTest : UserDatabaseTestHelper
{
    [Fact]
    public async Task SuccessfullyGetsToken()
    {
        var user = await _userRepository.GetUserAuthFromToken("token fdsa");
        Assert.True(user != null && user.Token == "token fdsa");
    }

    [Fact]
    public async Task FailsAsTokenDoesntExistInDatabase()
    {
        Assert.Null(await _userRepository.GetUserAuthFromToken("rea"));
    }
}
