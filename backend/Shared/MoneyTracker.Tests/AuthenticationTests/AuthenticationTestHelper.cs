
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using Moq;

namespace MoneyTracker.Tests.AuthenticationTests.Service;
public class AuthenticationTestHelper
{
    protected Mock<IIdGenerator> _mockIdGenerator = new();
    protected Mock<IDateTimeProvider> _mockDateTimeProvider = new();
    protected Mock<SecurityTokenHandler> _mockJwtTokenCreator = new();
    protected string _jwtConfigIss = "iss_company a";
    protected string _jwtConfigAud = "aud_company b";
    protected string _jwtConfigKey = "TOPSECRETTOPSECRETTOPSECRETTOPSE";
    protected int _jwtConfigExp = 15;

    protected AuthenticationService _authenticationService;

    public AuthenticationTestHelper()
    {
        var jwtToken = new JwtConfig(_jwtConfigIss,
            _jwtConfigAud,
            _jwtConfigKey,
            _jwtConfigExp
        );

        _authenticationService = new AuthenticationService(
            jwtToken,
            _mockDateTimeProvider.Object,
            _mockIdGenerator.Object,
            _mockJwtTokenCreator.Object);
    }

    public void EnsureAllMocksHadNoOtherCalls()
    {
        _mockDateTimeProvider.VerifyNoOtherCalls();
        _mockIdGenerator.VerifyNoOtherCalls();
        _mockJwtTokenCreator.VerifyNoOtherCalls();
    }
}