using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests;
public sealed class DecodeTokenTest
{
    [Fact]
    public void SuccessfullyDecodeToken()
    {
        var userId = Guid.NewGuid().ToString();
        var tempJti = Guid.NewGuid();
        var jwtConfigIss = "iss_company a";
        var jwtConfigAud = "aud_company b";
        var jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
        var jwtConfigExp = 15;
        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(jwtConfigExp);
        var tokenToDecode = "DeToken to dcode";

        var expected = new UserIdentity(userId);

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        var mockPasswordHasher = new Mock<IPasswordHasher>();

        var mockIdGenerator = new Mock<IIdGenerator>();
        
        ///////////////
        var claims = new[]
        {
            new Claim("UserGuid", userId),
            new Claim(JwtRegisteredClaimNames.Jti, tempJti.ToString())
        };
        // Create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Create JWT token
        var token = new JwtSecurityToken(
            issuer: jwtConfigIss,
            audience: jwtConfigAud,
            claims: claims,
            expires: dateTimeExp,
            signingCredentials: creds
        );
        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();
        mockJwtTokenCreator.Setup(x => x.ReadToken(tokenToDecode)).Returns(token);
        ///////////////

        var jwtToken = new JwtConfig(jwtConfigIss,
            jwtConfigAud,
            jwtConfigKey,
            jwtConfigExp
        );

        var userAuthService = new AuthenticationService(jwtToken,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object,
            mockJwtTokenCreator.Object);

        Assert.Multiple(() =>
        {
            Assert.Equal(expected, userAuthService.DecodeToken(tokenToDecode));

            mockJwtTokenCreator.Verify(x => x.ReadToken(tokenToDecode), Times.Once);
        });
    }

    [Fact]
    public void FailToDecodeTokenDueToTokenItselfBeingTooOld()
    {
        var userId = 1;
        var tempToken = Guid.NewGuid();
        var tempJti = Guid.NewGuid();
        var jwtConfigIss = "iss_company a";
        var jwtConfigAud = "aud_company b";
        var jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
        var jwtConfigExp = 15;
        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(jwtConfigExp);
        var tokenToDecode = "DeToken to dcode";

        var expected = new AuthenticatedUser(userId);

        var mockIdGenerator = new Mock<IIdGenerator>();

        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        var mockUserDb = new Mock<IUserAuthRepository>();

        var mockPasswordHasher = new Mock<IPasswordHasher>();

        ///////////////
        var claims = new[]
        {
            new Claim("UserGuid", tempToken.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, tempJti.ToString())
        };
        // Create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Create JWT token
        var token = new JwtSecurityToken(
            issuer: jwtConfigIss,
            audience: jwtConfigAud,
            claims: claims,
            expires: dateTimeNow.AddMinutes(-5),
            signingCredentials: creds
        );
        var mockJwtTokenCreator = new Mock<SecurityTokenHandler>();
        mockJwtTokenCreator.Setup(x => x.ReadToken(tokenToDecode)).Returns(token);
        ///////////////

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
            var error = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            {
                await userAuthService.DecodeToken(tokenToDecode);
            });
            Assert.Equal("Token has expired!", error.Message);

            mockJwtTokenCreator.Verify(x => x.ReadToken(tokenToDecode), Times.Once);
        });
    }
}
