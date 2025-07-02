
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public sealed class IsTokenValidTest : UserTestHelper
{
    [Fact]
    public async Task TokenIsValid()
    {
        var token = "Afds";
        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.CheckValidation())
            .Returns(Result.Success());

        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token, CancellationToken.None))
            .ReturnsAsync(mockUserAuth.Object);

        Assert.True(await _userService.IsTokenValid(token, CancellationToken.None));
    }

    [Fact]
    public async Task TokenReturnsNull()
    {
        var token = "Afds";
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token, CancellationToken.None))
            .ReturnsAsync((UserAuthentication?)null);

        Assert.False(await _userService.IsTokenValid(token, CancellationToken.None));
    }

    [Fact]
    public async Task TokenFailsValidation()
    {
        var mockUserAuth = new Mock<IUserAuthentication>();
        mockUserAuth.Setup(x => x.CheckValidation())
            .Returns(Result.Failure(Error.Validation("", "")));

        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(mockUserAuth.Object);

        Assert.False(await _userService.IsTokenValid(It.IsAny<string>(), CancellationToken.None));
    }
}
