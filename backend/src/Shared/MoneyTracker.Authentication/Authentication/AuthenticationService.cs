using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Authentication.Authentication;
public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtConfig _jwtToken;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIdGenerator _idGenerator;
    private readonly SecurityTokenHandler _securityTokenHandler;

    public AuthenticationService(IJwtConfig jwtConfig,
        IDateTimeProvider dateTimeProvider,
        IIdGenerator idGenerator,
        SecurityTokenHandler securityTokenHandler)
    {
        _jwtToken = jwtConfig;
        _dateTimeProvider = dateTimeProvider;
        _idGenerator = idGenerator;
        _securityTokenHandler = securityTokenHandler;
    }

    public string GenerateToken(UserIdentity user, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim("UserGuid", user.Identifier),
            new Claim(JwtRegisteredClaimNames.Jti, _idGenerator.NewGuid.ToString())
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

        return _securityTokenHandler.WriteToken(token);
    }

    public UserIdentity DecodeToken(string token)
    {
        var data = _securityTokenHandler.ReadToken(token);

        var tokenExpiryDate = data.ValidTo;
        if (tokenExpiryDate < _dateTimeProvider.Now)
        {
            throw new InvalidDataException("Token has expired!");
        }

        var userGuid = ((JwtSecurityToken)data).Claims.First(claim => claim.Type == "UserGuid");

        return new UserIdentity(userGuid.Value);
    }
}
