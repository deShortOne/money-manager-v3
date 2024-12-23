using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests.Service;
public sealed class GenerateTokenTest
{
    [Fact]
    public void SuccessfullyGenerateATokenForUser()
    {
        var userId = Guid.NewGuid().ToString();
        var tempJti = Guid.NewGuid();
        var jwtConfigIss = "iss_company a";
        var jwtConfigAud = "aud_company b";
        var jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
        var jwtConfigExp = 15;
        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(jwtConfigExp);

        var userToAuthenticate = new UserIdentity(userId);

        var mockIdGenerator = new Mock<IIdGenerator>();
        mockIdGenerator.SetupSequence(x => x.NewGuid)
            .Returns(tempJti);

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Expression<Func<JwtSecurityToken, bool>> checkSomeValuesOfJwtSecurityToken = x =>
            x.Claims.FirstOrDefault(y => y.Type == "UserGuid").Value == userId &&
            x.Claims.FirstOrDefault(y => y.Type == JwtRegisteredClaimNames.Jti).Value == tempJti.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();
        mockJwtTokenCreator.Setup(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken))).Returns("Successfully tokenised");

        var jwtToken = new JwtConfig(jwtConfigIss,
            jwtConfigAud,
            jwtConfigKey,
            jwtConfigExp
        );

        var userAuthService = new AuthenticationService(
            jwtToken,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object,
            mockJwtTokenCreator.Object);

        Assert.Multiple(() =>
        {
            Assert.Equal("Successfully tokenised", userAuthService.GenerateToken(userToAuthenticate, new DateTime()));

            mockDateTimeProvider.Verify(x => x.Now, Times.Once);
            mockIdGenerator.Verify(x => x.NewGuid, Times.Once);
            mockJwtTokenCreator.Verify(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken)), Times.Once);
        });
    }
}
