using MoneyTracker.Authentication.Entities;
using Moq;

namespace MoneyTracker.Queries.Tests.UserTests.Service;
public sealed class GetLastUserTokenForUserTest : UserTestHelper
{
    private int _userId = 1;
    private string _username = "";
    private string _passwordFromDb = "";
    private string _passwordFromUser = "";

    [Fact]
    public void SuccessfullyGetToken()
    {
        var user = new UserEntity(_userId, _username, _passwordFromDb);

        _mockUserDatabase.Setup(x => x.GetUserByUsername(_username, CancellationToken.None))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(x => x.VerifyPassword(_passwordFromDb, _passwordFromUser)).Returns(true);

        _mockUserDatabase.Setup(x => x.GetLastUserTokenForUser(user, CancellationToken.None)).ReturnsAsync("a new token");

        Assert.Multiple(async () =>
        {
            await _userService.GetUserToken(new Authentication.DTOs.LoginWithUsernameAndPassword(_username, _passwordFromUser), CancellationToken.None);

            _mockUserDatabase.Verify(x => x.GetUserByUsername(_username, CancellationToken.None), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(_passwordFromDb, _passwordFromUser), Times.Once);
            _mockUserDatabase.Verify(x => x.GetLastUserTokenForUser(user, CancellationToken.None), Times.Once);
            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
