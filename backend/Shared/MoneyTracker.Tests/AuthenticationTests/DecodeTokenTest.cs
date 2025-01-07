using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests.Service;
public sealed class DecodeTokenTest : AuthenticationTestHelper
{
    [Fact]
    public void SuccessfullyDecodeToken()
    {
        var userId = Guid.NewGuid().ToString();
        var tempJti = Guid.NewGuid();

        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(_jwtConfigExp);
        var tokenToDecode = "DeToken to dcode";

        var expected = new UserIdentity(userId);

        _mockDateTimeProvider.Setup(x => x.Now).Returns(dateTimeNow);

        ///////////////
        var claims = new[]
        {
            new Claim("UserGuid", userId),
            new Claim(JwtRegisteredClaimNames.Jti, tempJti.ToString())
        };
        // Create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        // Create JWT token
        var token = new JwtSecurityToken(
            issuer: _jwtConfigIss,
            audience: _jwtConfigAud,
            claims: claims,
            expires: dateTimeExp,
            signingCredentials: creds
        );
        _mockJwtTokenCreator.Setup(x => x.ReadToken(tokenToDecode)).Returns(token);
        ///////////////

        Assert.Multiple(() =>
        {
            Assert.Equal(expected, _authenticationService.DecodeToken(tokenToDecode));

            _mockJwtTokenCreator.Verify(x => x.ReadToken(tokenToDecode), Times.Once);
        });
    }
}
