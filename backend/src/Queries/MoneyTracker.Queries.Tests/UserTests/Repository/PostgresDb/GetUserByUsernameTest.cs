using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.PostgresDb;
public sealed class GetUserByUsernameTest : UserDatabaseTestHelper
{
    public GetUserByUsernameTest(PostgresDbFixture postgresFixture) : base(postgresFixture)
    {

    }

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
