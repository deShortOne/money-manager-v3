
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

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserId.ToString()),
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
            expires: DateTime.Now.AddMinutes(_jwtToken.Expires),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AuthenticatedUser DecodeToken(string token)
    {
        var data = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var sub = data.Claims.First(claim => claim.Type == "sub");

        return new AuthenticatedUser(int.Parse(sub.Value));
    }
}
