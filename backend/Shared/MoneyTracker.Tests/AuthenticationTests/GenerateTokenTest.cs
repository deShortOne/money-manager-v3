using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests.Service;
public sealed class GenerateTokenTest : AuthenticationTestHelper
{
    [Fact]
    public void SuccessfullyGenerateATokenForUser()
    {
        var userId = Guid.NewGuid().ToString();
        var tempJti = Guid.NewGuid();

        var dateTimeNow = new DateTime(2024, 10, 6, 9, 0, 0, DateTimeKind.Utc);
        var dateTimeExp = dateTimeNow.AddMinutes(_jwtConfigExp);

        var userToAuthenticate = new UserIdentity(userId);

        _mockIdGenerator.SetupSequence(x => x.NewGuid)
            .Returns(tempJti);


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Expression<Func<JwtSecurityToken, bool>> checkSomeValuesOfJwtSecurityToken = x =>
            x.Claims.FirstOrDefault(y => y.Type == "UserGuid").Value == userId &&
            x.Claims.FirstOrDefault(y => y.Type == JwtRegisteredClaimNames.Jti).Value == tempJti.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        _mockJwtTokenCreator.Setup(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken))).Returns("Successfully tokenised");

        Assert.Multiple(() =>
        {
            Assert.Equal("Successfully tokenised", _authenticationService.GenerateToken(userToAuthenticate, new DateTime()));

            _mockIdGenerator.Verify(x => x.NewGuid, Times.Once);
            _mockJwtTokenCreator.Verify(x => x.WriteToken(It.Is(checkSomeValuesOfJwtSecurityToken)), Times.Once);
            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
