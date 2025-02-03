
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public sealed class LoginUserTest : UserTestHelper
{
    private int _userId = 2;
    private string _username = "jkl";
    private string _passwordFromLogin = "eior";
    private string _passwordFromDatabase = "er";
    private DateTime _timeNow = new(2024, 3, 23, 3, 34, 11);
    private DateTime _timeExpire = new(2024, 3, 23, 4, 34, 11); // magic value
    private Guid _newToken = Guid.NewGuid();

    [Fact]
    public void SuccessfullyLoginUser()
    {
        var user = new UserEntity(_userId, _username, _passwordFromDatabase);
        var userIdentity = new UserIdentity(_userId.ToString());
        var userAuthentication = new UserAuthentication(user, _newToken.ToString(), _timeExpire, _mockDateTimeProvider.Object);

        _mockUserDatabase.Setup(x => x.GetUserByUsername(_username))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(_passwordFromDatabase, _passwordFromLogin))
            .Returns(true);
        _mockAuthService.Setup(x => x.GenerateToken(userIdentity, _timeExpire)).Returns(_newToken.ToString());
        _mockDateTimeProvider.Setup(x => x.Now).Returns(_timeNow);
        _mockUserDatabase.Setup(x => x.StoreTemporaryTokenToUser(userAuthentication));

        Assert.Multiple(async () =>
        {
            await _userService.LoginUser(new LoginWithUsernameAndPassword(_username, _passwordFromLogin));

            _mockUserDatabase.Verify(x => x.GetUserByUsername(_username), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(_passwordFromDatabase, _passwordFromLogin), Times.Once);
            _mockDateTimeProvider.Verify(x => x.Now, Times.Once);
            _mockAuthService.Verify(x => x.GenerateToken(userIdentity, _timeExpire), Times.Once);
            _mockUserDatabase.Verify(x => x.StoreTemporaryTokenToUser(userAuthentication), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                new EventUpdate(new AuthenticatedUser(_userId), DataTypes.User), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
