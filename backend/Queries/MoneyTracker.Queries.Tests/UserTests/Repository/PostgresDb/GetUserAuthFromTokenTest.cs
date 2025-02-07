using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.PostgresDb;
public sealed class GetUserAuthFromTokenTest : UserDatabaseTestHelper
{
    public GetUserAuthFromTokenTest(PostgresDbFixture postgresFixture) : base(postgresFixture)
    {

    }

    [Fact]
    public async Task SuccessfullyGetsToken()
    {
        var user = await _userRepository.GetUserAuthFromToken("token 2");
        Assert.True(user != null && user.Token == "token 2");
    }

    [Fact]
    public async Task FailsAsTokenDoesntExistInDatabase()
    {
        Assert.Null(await _userRepository.GetUserAuthFromToken("rea"));
    }
}
