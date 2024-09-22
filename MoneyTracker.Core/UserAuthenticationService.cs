
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
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

    public Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword user)
    {
        return _dbService.AuthenticateUser(user);
    }

    public async Task<string> GenerateToken(LoginWithUsernameAndPassword user)
    {
        var userInfo = await _dbService.AuthenticateUser(user);

        var expiration = DateTime.Now.AddMinutes(_jwtToken.Expires);
        var userGuid = await _dbService.GenerateTempGuidForUser(userInfo, expiration);

        var claims = new[]
        {
            new Claim("UserGuid", userGuid.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Create signing key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtToken.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create JWT token
        var token = new JwtSecurityToken(
            issuer: _jwtToken.Issuer,
            audience: _jwtToken.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<AuthenticatedUser> DecodeToken(string token)
    {
        var data = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var tokenExpiryDate = data.ValidTo;
        if (tokenExpiryDate < new DateTime())
        {
            throw new InvalidDataException("Token has expired!");
        }

        var userGuid = data.Claims.First(claim => claim.Type == "UserGuid");
        var userInfoFromDb = await _dbService.GetUserFromGuid(Guid.Parse(userGuid.Value));

        if (userInfoFromDb.Expires < new DateTime())
        {
            throw new InvalidDataException("Invalid token: expired");
        }

        return new AuthenticatedUser(userInfoFromDb.UserId);
    }
}
