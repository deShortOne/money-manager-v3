namespace MoneyTracker.Queries.Tests.UserTests.Repository.Postgres;
public sealed class GetUserByUsernameTest : UserDatabaseTestHelper
{
    [Fact]
    public async Task SuccessfullyGetsAValidUser()
    {
        Assert.NotNull(await _userRepository.GetUserByUsername("root"));
    }

    [Fact]
    public async Task FailsToGetsAnInvalidUser()
    {
        Assert.Null(await _userRepository.GetUserByUsername("fdashg"));
    }
}
