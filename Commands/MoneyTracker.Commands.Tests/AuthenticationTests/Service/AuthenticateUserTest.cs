using System.IdentityModel.Tokens.Jwt;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Authentication.Utils;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests.Service;
public class AuthenticateUserTest
{
    [Fact]
    public void SuccessfullyLogInUser1()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-pass");
        var expected = new AuthenticatedUser(1);

        var mockUserDb = new Mock<IUserAuthRepository>();
        mockUserDb.Setup(x => x.GetUserByUsername("root"))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(1, "root", "root-pass")));

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword("root-pass", "root-pass", "salt goes here"))
            .Returns(true);

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(mockUserDb.Object,
            jwtToken,
            new DateTimeProvider(),
            mockPasswordHasher.Object,
            new IdGenerator(),
            new JwtSecurityTokenHandler());

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));

            mockUserDb.Verify(x => x.GetUserByUsername("root"), Times.Once);
            mockPasswordHasher.Verify(x => x.VerifyPassword("root-pass", "root-pass", "salt goes here"), Times.Once);
        });
    }

    [Fact]
    public void SuccessfullyLogInUser2()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("secondary root", "secondary root-pass");
        var expected = new AuthenticatedUser(2);

        var mockUserDb = new Mock<IUserAuthRepository>();
        mockUserDb.Setup(x => x.GetUserByUsername(It.Is<string>(y => y == "secondary root")))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(2, "secondary root", "secondary root-pass")));

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword("secondary root-pass", "secondary root-pass", "salt goes here"))
            .Returns(true);

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(mockUserDb.Object,
            jwtToken,
            new DateTimeProvider(),
            mockPasswordHasher.Object,
            new IdGenerator(),
            new JwtSecurityTokenHandler());

        Assert.Multiple(async () =>
        {
            Assert.Equal(expected, await userAuthService.AuthenticateUser(userToAuthenticate));

            mockUserDb.Verify(x => x.GetUserByUsername("secondary root"), Times.Once);
            mockPasswordHasher.Verify(x => x.VerifyPassword("secondary root-pass", "secondary root-pass", "salt goes here"), Times.Once);
        });
    }

    [Fact]
    public void User1_CorrectUsernameWrongPassword_FailsToAuthenticateUser()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-");

        var mockUserDb = new Mock<IUserAuthRepository>();
        mockUserDb.Setup(x => x.GetUserByUsername("root"))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(1, "root", "root-pass")));

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword("root-pass", "root-", "salt goes here"))
            .Returns(false);

        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(mockUserDb.Object,
            jwtToken,
            new DateTimeProvider(),
            mockPasswordHasher.Object,
            new IdGenerator(),
            new JwtSecurityTokenHandler());

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await userAuthService.AuthenticateUser(userToAuthenticate);
            });
            Assert.Equal("User does not exist", error.Message);

            mockUserDb.Verify(x => x.GetUserByUsername("root"), Times.Once);
            mockPasswordHasher.Verify(x => x.VerifyPassword("root-pass", "root-", "salt goes here"), Times.Once);
        });
    }

    [Fact]
    public void FailToLogInUserThatDoesntExist()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("broken root", "broken root-pass");

        var userDb = new Mock<IUserAuthRepository>();
        userDb.Setup(x => x.GetUserByUsername(It.IsAny<string>()))
            .Returns(Task.FromResult<UserEntity?>(null));
        var jwtToken = new JwtConfig("", "", "", 0);
        var userAuthService = new UserAuthenticationService(userDb.Object,
            jwtToken,
            new DateTimeProvider(),
            new PasswordHasher(),
            new IdGenerator(),
            new JwtSecurityTokenHandler());

        Assert.Multiple(async () =>
        {
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await userAuthService.AuthenticateUser(userToAuthenticate);
            });

            Assert.Equal("User does not exist", error.Message);
        });
    }
}
