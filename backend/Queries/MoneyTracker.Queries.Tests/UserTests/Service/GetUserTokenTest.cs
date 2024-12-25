using MoneyTracker.Authentication.Entities;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public sealed class GetUserTokenTest : UserTestHelper
{
    private int _userId = 1;
    private string _username = "";
    private string _passwordFromDb = "";
    private string _passwordFromUser = "";

    [Fact]
    public void SuccessfullyGetToken()
    {
        var user = new UserEntity(_userId, _username, _passwordFromDb);

        _mockUserDatabase.Setup(x => x.GetUserByUsername(_username))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(x => x.VerifyPassword(_passwordFromDb, _passwordFromUser, "")).Returns(true);

        _mockUserDatabase.Setup(x => x.GetUserToken(user)).ReturnsAsync("a new token");

        Assert.Multiple(async () =>
        {
            await _userService.GetUserToken(new Authentication.DTOs.LoginWithUsernameAndPassword(_username, _passwordFromUser));

            _mockUserDatabase.Verify(x => x.GetUserByUsername(_username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(_passwordFromDb, _passwordFromUser, ""), Times.Once);
            _mockUserDatabase.Verify(x => x.GetUserToken(user), Times.Once);
            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
