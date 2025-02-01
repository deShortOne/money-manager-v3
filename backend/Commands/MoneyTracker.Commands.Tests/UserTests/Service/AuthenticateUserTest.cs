using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public class AuthenticateUserTest : UserTestHelper
{
    [Fact]
    public void SuccessfullyLogInUser1()
    {
        var userFromDb = new UserEntity(1, "root", "root-pass");
        var dateTimeNow = new DateTime(2024, 2, 2, 15, 0, 0);
        var dateTimeExp = new DateTime(2024, 2, 2, 16, 0, 0); // magic value

        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-pass");

        _mockUserDatabase.Setup(x => x.GetUserByUsername("root"))
            .Returns(Task.FromResult(userFromDb));

        _mockPasswordHasher.Setup(x => x.VerifyPassword("root-pass", "root-pass", ""))
            .Returns(true);

        _mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        _mockAuthService.Setup(x => x.GenerateToken(new UserIdentity("1"), dateTimeExp)).Returns("ASDF");

        Assert.Multiple(async () =>
        {
            await _userService.LoginUser(userToAuthenticate);

            _mockUserDatabase.Verify(x => x.GetUserByUsername("root"), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword("root-pass", "root-pass", ""), Times.Once);
            _mockUserDatabase.Verify(x => x.StoreTemporaryTokenToUser(new UserAuthentication(userFromDb, "ASDF",
                dateTimeExp, _mockDateTimeProvider.Object)));
            _mockAuthService.Verify(x => x.GenerateToken(new UserIdentity("1"), dateTimeExp), Times.Once);
            _mockDateTimeProvider.Verify(x => x.Now, Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                new EventUpdate(new AuthenticatedUser(1), DataTypes.User), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public void SuccessfullyLogInUser2()
    {
        var userFromDb = new UserEntity(2, "secondary root", "secondary root-pass");
        var dateTimeNow = new DateTime(2024, 2, 2, 15, 0, 0);
        var dateTimeExp = new DateTime(2024, 2, 2, 16, 0, 0); // magic value

        var userToAuthenticate = new LoginWithUsernameAndPassword("secondary root", "secondary root-pass");

        _mockUserDatabase.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == "secondary root")))
            .Returns(Task.FromResult(userFromDb));

        _mockPasswordHasher.Setup(x => x.VerifyPassword("secondary root-pass", "secondary root-pass", ""))
            .Returns(true);

        _mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        _mockAuthService.Setup(x => x.GenerateToken(new UserIdentity("2"), dateTimeExp)).Returns("ASDFAA");

        Assert.Multiple(async () =>
        {
            await _userService.LoginUser(userToAuthenticate);

            _mockUserDatabase.Verify(x => x.GetUserByUsername("secondary root"), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword("secondary root-pass", "secondary root-pass", ""), Times.Once);
            _mockUserDatabase.Verify(x => x.StoreTemporaryTokenToUser(new UserAuthentication(userFromDb, "ASDFAA",
                dateTimeExp, _mockDateTimeProvider.Object)));
            _mockAuthService.Verify(x => x.GenerateToken(new UserIdentity("2"), dateTimeExp), Times.Once);
            _mockDateTimeProvider.Verify(x => x.Now, Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                new EventUpdate(new AuthenticatedUser(2), DataTypes.User), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task User1_CorrectUsernameWrongPassword_FailsToAuthenticateUser()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-");

        _mockUserDatabase.Setup(x => x.GetUserByUsername("root"))
            .Returns(Task.FromResult(new UserEntity(1, "root", "root-pass")));

        _mockPasswordHasher.Setup(x => x.VerifyPassword("root-pass", "root-", ""))
            .Returns(false);

        var result = await _userService.LoginUser(userToAuthenticate);
        Assert.Multiple(() =>
        {
            Assert.Equal("User does not exist", result.Error.Description);

            _mockUserDatabase.Verify(x => x.GetUserByUsername("root"), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword("root-pass", "root-", ""), Times.Once);
            EnsureAllMocksHadNoOtherCalls();
        });
    }

    [Fact]
    public async Task FailToLogInUserThatDoesntExist_()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("broken root", "broken root-pass");

        _mockUserDatabase.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
            .Returns(Task.FromResult<UserEntity>(null));

        var result = await _userService.LoginUser(userToAuthenticate);
        Assert.Multiple(() =>
        {
            Assert.Equal("User does not exist", result.Error.Description);

            _mockUserDatabase.Verify(x => x.GetUserByUsername(It.IsAny<string>()), Times.Once);
            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
