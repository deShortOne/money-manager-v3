
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public sealed class GetUserFromTokenTest : UserTestHelper
{
    [Fact]
    public async Task SuccessfullyGetUserFromToken()
    {
        var userId = 44;
        var user = new UserEntity(userId, "", "");

        var token = "asdf";
        var userAuthFromRepository = new UserAuthentication(user, token, new DateTime(2025, 1, 11), _mockDateTimeProvider.Object);

        var expected = ResultT<AuthenticatedUser>.Success(new AuthenticatedUser(userId));

        _mockDateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2025, 1, 10));
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(token, CancellationToken.None)).ReturnsAsync(userAuthFromRepository);

        var result = await _userService.GetUserFromToken(token, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Fail_TokenDoesntReturnUserAuth()
    {
        var token = "asdf";

        var expected = ResultT<AuthenticatedUser>
            .Failure(Error.AccessUnAuthorised("UserService.GetUserFromToken", "Token not found"));

        _mockDateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2025, 1, 10));
        _mockUserDatabase.Setup(x => x.GetUserAuthFromToken(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((UserAuthentication)null);

        var result = await _userService.GetUserFromToken(token, CancellationToken.None);

        Assert.Equal(expected, result);
        //ResultExtensions.Equals(expected, result);
    }
}
