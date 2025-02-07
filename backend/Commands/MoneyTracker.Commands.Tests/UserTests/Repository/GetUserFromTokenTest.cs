using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Commands.Tests.Fixture;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Repository;
public sealed class GetUserFromTokenTest : IClassFixture<PostgresDbFixture>
{
    private UserCommandRepository _userAuthRepo;
    private Mock<DateTimeProvider> _mockDateTimeProvider;

    public GetUserFromTokenTest(PostgresDbFixture postgresFixture)
    {
        _mockDateTimeProvider = new Mock<DateTimeProvider>();

        var database = new PostgresDatabase(postgresFixture.ConnectionString);
        _userAuthRepo = new UserCommandRepository(database, _mockDateTimeProvider.Object);
    }

    [Fact]
    public async Task SuccessfullyLogInUser()
    {
        var expected = new UserAuthentication(new(1, "root", "IfC1pbsUdKwcX68HPvPybQ==.bfXuHix96vvlXfGqLpY+/kRgBnCbXCU/Kqu2uIY8M60="), "token 1", new DateTime(2025, 2, 3, 23, 24, 13, 126, 961), _mockDateTimeProvider.Object);

        var actual = await _userAuthRepo.GetUserAuthFromToken("token 1");
        Assert.Equal(expected, actual);
    }


    [Fact]
    public async Task ReturnNullForIncorrectToken()
    {
        var actual = await _userAuthRepo.GetUserAuthFromToken(Guid.NewGuid().ToString());
        Assert.Null(actual);
    }
}
