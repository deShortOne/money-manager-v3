
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public sealed class IsTokenValidTest : UserTestHelper
{
    [Fact]
    public async Task TokenIsValid()
    {
        var token = "Afds";
        var user = new UserEntity(1, "", "");
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2024, 1, 12));
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token))
            .ReturnsAsync(new UserAuthentication(user, token, new DateTime(2024, 1, 13), dateTimeProvider.Object));

        Assert.True(await _userService.IsTokenValid(token));
    }

    [Fact]
    public async Task TokenReturnsNull()
    {
        var token = "Afds";
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token))
            .ReturnsAsync((UserAuthentication?)null);

        Assert.False(await _userService.IsTokenValid(token));
    }

    [Fact]
    public async Task TokenFailsValidation()
    {
        var token = "Afds";
        var user = new UserEntity(1, "", "");
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2024, 1, 14));
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token))
            .ReturnsAsync(new UserAuthentication(user, token, new DateTime(2024, 1, 13), dateTimeProvider.Object));

        Assert.False(await _userService.IsTokenValid(token));
    }
}
