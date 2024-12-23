
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public sealed class LoginUserTest : UserTestHelper
{
    private int _userId = 2;
    private string _username = "jkl";
    private string _passwordFromLogin = "eior";
    private string _passwordFromDatabase = "eior";
    private DateTime _timeNow = new(23, 3, 2024, 3, 34, 11);
    private DateTime _timeExpire = new(23, 3, 2024, 4, 34, 11); // magic value
    private Guid _newToken = Guid.NewGuid();

    [Fact]
    public async Task SuccessfullyLoginUser()
    {
        var user = new UserEntity(_userId, _username, _passwordFromDatabase);
        var userIdentity = new UserIdentity(_newToken.ToString());

        _mockUserDatabase.Setup(x => x.GetUserByUsername(_username))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(_passwordFromLogin, _passwordFromDatabase, ""))
            .Returns(true);
        _mockIdGenerator.Setup(x => x.NewGuid).Returns(_newToken);
        _mockDateTimeProvider.Setup(x => x.Now).Returns(_timeNow);
        _mockAuthService.Setup(x => x.GenerateToken(userIdentity, _timeExpire));
        _mockUserDatabase.Setup(x => x.StoreTemporaryTokenToUser(user, _newToken.ToString(), _timeExpire));
        
        await _userService.AddNewUser(new LoginWithUsernameAndPassword(_username, _passwordFromLogin));

        _mockUserDatabase.Verify(x => x.GetUserByUsername(_username), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(_passwordFromLogin, _passwordFromDatabase, ""), Times.Once);
        _mockIdGenerator.Verify(x => x.NewGuid, Times.Once);
        _mockDateTimeProvider.Verify(x => x.Now, Times.Once);
        _mockAuthService.Verify(x => x.GenerateToken(userIdentity, _timeExpire), Times.Once);
        _mockUserDatabase.Verify(x => x.StoreTemporaryTokenToUser(user, _newToken.ToString(), _timeExpire), Times.Once);
        EnsureAllMocksHadNoOtherCalls();
    }
}
