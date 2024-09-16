
using System.Text;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;

namespace MoneyTracker.Core;
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IUserAuthDatabase _dbService;
    private readonly IJwtConfig _jwtToken;

    public UserAuthenticationService(IUserAuthDatabase dbService, IJwtConfig jwtConfig)
    {
        _dbService = dbService;
        _jwtToken = jwtConfig;
    }

    public object JwtRegisteredClaimNames { get; private set; }

    public Task<AuthenticatedUser> AuthenticateUser(UnauthenticatedUser user)
    {
        return _dbService.AuthenticateUser(user);
    }
}
