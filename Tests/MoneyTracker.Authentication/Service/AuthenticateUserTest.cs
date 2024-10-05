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
        var userToAuthenticate = new LoginWithUsernameAndPassword("root");
        var expected = new AuthenticatedUser(1);

        var userDb = new Mock<IUserAuthDatabase>();
        userDb.Setup(x => x.AuthenticateUser(It.Is<LoginWithUsernameAndPassword>(y => y == userToAuthenticate)))
            .Returns(Task.FromResult(expected));

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider());

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void SuccessfullyLogInUser2()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("secondary root");
        var expected = new AuthenticatedUser(2);

        var userDb = new Mock<IUserAuthDatabase>();
        userDb.Setup(x => x.AuthenticateUser(It.Is<LoginWithUsernameAndPassword>(y => y == userToAuthenticate)))
            .Returns(Task.FromResult(expected));

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider());

        Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));
    }

    [Fact]
    public async void FailToLogInUserThatDoesntExist()
    {
        //var userToAuthenticate = new LoginWithUsernameAndPassword("broken root");

        //var userDb = new Mock<IUserAuthDatabase>();
        //var jwtToken = new JwtConfig("", "", "", 0);
        //var userAuthService = new UserAuthenticationService(userDb.Object, jwtToken, new DateTimeProvider());

        //await Assert.ThrowsAsync<InvalidDataException>(async () =>
        //{
        //    await userAuthService.AuthenticateUser(userToAuthenticate);
        //});
    }
}
