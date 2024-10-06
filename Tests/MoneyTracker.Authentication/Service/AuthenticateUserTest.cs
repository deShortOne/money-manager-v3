using MoneyTracker.Core;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using Moq;

namespace MoneyTracker.Authentication.Service;

public class AuthenticateUserTest
{
    [Fact]
    public async void SuccessfullyLogInUser1()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-pass");
        var expected = new AuthenticatedUser(1);

        var userDb = new Mock<IUserAuthDatabase>();
        userDb.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == "root")))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(1, "root", "root-pass")));

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider(),
            new PasswordHasher());

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void SuccessfullyLogInUser2()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("secondary root", "secondary root-pass");
        var expected = new AuthenticatedUser(2);

        var userDb = new Mock<IUserAuthDatabase>();
        userDb.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == "secondary root")))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(2, "secondary root", "secondary-root")));

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider(),
            new PasswordHasher());

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void FailToLogInUserThatDoesntExist()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("broken root", "broken root-pass");

        var userDb = new Mock<IUserAuthDatabase>();
        userDb.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
            .Returns(Task.FromResult<UserEntity?>(null));
        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider(),
            new PasswordHasher());

        var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await userAuthService.AuthenticateUser(userToAuthenticate);
        });
        Assert.Equal("User does not exist", error.Message);
    }
}
