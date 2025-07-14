using MoneyTracker.Authentication.Entities;
using MoneyTracker.Queries.Tests.Fixture;

namespace MoneyTracker.Queries.Tests.UserTests.Repository.PostgresDb;
public sealed class GetLastUserTokenForUserTest : UserDatabaseTestHelper
{
    public GetLastUserTokenForUserTest(PostgresDbFixture postgresFixture) : base(postgresFixture)
    {

    }

    [Fact]
    public async Task SuccessfullyGetLastToken()
    {
        Assert.Equal("token 2", await _userRepository.GetLastUserTokenForUser(new UserEntity(1, "", ""), CancellationToken.None));
    }

    [Fact]
    public async Task FailToGetTokenForUsersNotInDb()
    {
        Assert.Null(await _userRepository.GetLastUserTokenForUser(new UserEntity(2, "", ""), CancellationToken.None));
    }
}
