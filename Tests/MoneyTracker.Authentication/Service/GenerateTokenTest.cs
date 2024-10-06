
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Core;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Shared;
using Moq;

namespace MoneyTracker.Authentication.Service;
public sealed class GenerateTokenTest
{
    [Fact]
    public void SuccessfullyGenerateAndDecodeTokenForUser()
    {
        var userToAuthenticate = new LoginWithUsernameAndPassword("root", "root-pass");
        var expected = new AuthenticatedUser(1);

        var tempToken = Guid.NewGuid();
        var tempJti = Guid.NewGuid();
        var mockIdGenerator = new Mock<IIdGenerator>();
        mockIdGenerator.SetupSequence(x => x.NewGuid)
            .Returns(tempToken)
            .Returns(tempJti);

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(x => x.Now).Returns(new DateTime(2024, 10, 06, 9, 0, 0));

        var mockUserDb = new Mock<IUserAuthDatabase>();
        mockUserDb.Setup(x => x.GetUserByUsername("root"))
            .Returns(Task.FromResult<UserEntity?>(new UserEntity(1, "root", "root-pass")));
        mockUserDb.Setup(x => x.StoreTemporaryTokenToUser(expected, tempToken, new DateTime(2024, 10, 06, 9, 15, 0)));

        var mockPasswordHasher = new Mock<IPasswordHasher>();
        mockPasswordHasher.Setup(x => x.VerifyPassword("root-pass", "root-pass", "salt goes here"))
            .Returns(true);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Expression<Func<JwtSecurityToken, bool>> checkSomeValuesOfJwtSecurityToken = x =>
            x.Claims.FirstOrDefault(y => y.Type == "UserGuid").Value == tempToken.ToString() &&
            x.Claims.FirstOrDefault(y => y.Type == JwtRegisteredClaimNames.Jti).Value == tempJti.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();
        mockJwtTokenCreator.Setup(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken))).Returns("Successfully tokenised");

        var jwtToken = new JwtConfig("iss_company a",
            "aud_company b",
            "TOPSECRETTOPSECRETTOPSECRETTOPSE",
            15
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

            mockUserDb.Verify(x => x.GetUserByUsername("root"), Times.Once);
            mockUserDb.Verify(x => x.StoreTemporaryTokenToUser(expected, tempToken, new DateTime(2024, 10, 06, 9, 15, 0)), Times.Once);
            mockPasswordHasher.Verify(x => x.VerifyPassword("root-pass", "root-pass", "salt goes here"), Times.Once);

            mockIdGenerator.Verify(x => x.NewGuid, Times.Exactly(2));
        });
    }
}
