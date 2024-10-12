
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Core;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Authentication.Tests.Service;
public sealed class GenerateTokenTest
{
    [Fact]
    public void SuccessfullyGenerateATokenForUser()
    {
        var userId = 1;
        var username = "root";
        var userProvidedpassword = "root-pass";
        var databasePassword = "root-pass";
        var tempToken = Guid.NewGuid();
        var tempJti = Guid.NewGuid();
        var jwtConfigIss = "iss_company a";
        var jwtConfigAud = "aud_company b";
        var jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
        var jwtConfigExp = 15;
        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(jwtConfigExp);

        var userToAuthenticate = new LoginWithUsernameAndPassword(username, userProvidedpassword);
        var expected = new AuthenticatedUser(userId);

        var mockIdGenerator = new Mock<IIdGenerator>();
        mockIdGenerator.SetupSequence(x => x.NewGuid)
            .Returns(tempToken)
            .Returns(tempJti);

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        var mockUserDb = new Mock<IUserAuthDatabase>();
        mockUserDb.Setup(x => x.GetUserByUsername(username))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(userId, username, databasePassword)));
        mockUserDb.Setup(x => x.StoreTemporaryTokenToUser(expected, tempToken, dateTimeExp));

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword(databasePassword, userProvidedpassword, "salt goes here"))
            .Returns(true);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Expression<Func<JwtSecurityToken, bool>> checkSomeValuesOfJwtSecurityToken = x =>
            x.Claims.FirstOrDefault(y => y.Type == "UserGuid").Value == tempToken.ToString() &&
            x.Claims.FirstOrDefault(y => y.Type == JwtRegisteredClaimNames.Jti).Value == tempJti.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();
        mockJwtTokenCreator.Setup(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken))).Returns("Successfully tokenised");

        var jwtToken = new JwtConfig(jwtConfigIss,
            jwtConfigAud,
            jwtConfigKey,
            jwtConfigExp
        );

        var userAuthService = new UserAuthenticationService(mockUserDb.Object,
            jwtToken,
            mockDateTimeProvider.Object,
            mockPasswordHasher.Object,
            mockIdGenerator.Object,
            mockJwtTokenCreator.Object);

        Assert.Multiple(async () =>
        {
            Assert.Equal("Successfully tokenised", await userAuthService.GenerateToken(userToAuthenticate));

            mockUserDb.Verify(x => x.GetUserByUsername(username), Times.Once);
            mockUserDb.Verify(x => x.StoreTemporaryTokenToUser(expected, tempToken, dateTimeExp), Times.Once);
            mockDateTimeProvider.Verify(x => x.Now, Times.Once);
            mockPasswordHasher.Verify(x => x.VerifyPassword(databasePassword, userProvidedpassword, "salt goes here"), Times.Once);
            mockIdGenerator.Verify(x => x.NewGuid, Times.Exactly(2));
            mockJwtTokenCreator.Verify(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken)), Times.Once);
        });
    }

    [Fact]
    public void FailToGenerateATokenForUserDueToNotBeingAbleToAuthenticateUser()
    {
        var userId = 1;
        var username = "root";
        var userProvidedpassword = "root-password";
        var databasePassword = "root-pass";
        var jwtConfigIss = "iss_company a";
        var jwtConfigAud = "aud_company b";
        var jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
        var jwtConfigExp = 15;

        var userToAuthenticate = new LoginWithUsernameAndPassword(username, userProvidedpassword);
        var expected = new AuthenticatedUser(userId);

        var mockIdGenerator = new Mock<IIdGenerator>();

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        var mockUserDb = new Mock<IUserAuthDatabase>();
        mockUserDb.Setup(x => x.GetUserByUsername(username))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(userId, username, databasePassword)));

        var mockPasswordHasher = new Mock<IPasswordHasher>();

        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();

        var jwtToken = new JwtConfig(jwtConfigIss,
            jwtConfigAud,
            jwtConfigKey,
            jwtConfigExp
        );

        var userAuthService = new UserAuthenticationService(mockUserDb.Object,
            jwtToken,
            mockDateTimeProvider.Object,
            mockPasswordHasher.Object,
            mockIdGenerator.Object,
            mockJwtTokenCreator.Object);

        Assert.Multiple(async () =>
        {
            await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await userAuthService.GenerateToken(userToAuthenticate);
            });
            mockUserDb.Verify(x => x.GetUserByUsername(username), Times.Once);
        });
    }
}
