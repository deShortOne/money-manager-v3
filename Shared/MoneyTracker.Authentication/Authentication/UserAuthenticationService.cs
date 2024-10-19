using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Authentication.Authentication;
public class UserAuthenticationService : IUserAuthenticationService
{
    private readonly IUserAuthRepository _dbService;
    private readonly IJwtConfig _jwtToken;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdGenerator _idGenerator;
    private readonly SecurityTokenHandler _securityTokenHandler;

    public UserAuthenticationService(IUserAuthRepository dbService,
        IJwtConfig jwtConfig,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher,
        IIdGenerator idGenerator,
        SecurityTokenHandler securityTokenHandler)
    {
        _dbService = dbService;
        _jwtToken = jwtConfig;
        _dateTimeProvider = dateTimeProvider;
        _passwordHasher = passwordHasher;
        _idGenerator = idGenerator;
        _securityTokenHandler = securityTokenHandler;
    }

    public async Task<AuthenticatedUser> AuthenticateUser(LoginWithUsernameAndPassword user)
    {
        var userEntity = await _dbService.GetUserByUsername(user.Username);
        if (userEntity == null)
        {
            throw new InvalidDataException("User does not exist");
        }
        if (!_passwordHasher.VerifyPassword(userEntity.Password, user.Password, "salt goes here"))
        {
            throw new InvalidDataException("User does not exist");
        }
        return new AuthenticatedUser(userEntity.Id);
    }

    public async Task<string> GenerateToken(LoginWithUsernameAndPassword user)
    {
        var userInfo = await AuthenticateUser(user);

        var expiration = _dateTimeProvider.Now.AddMinutes(_jwtToken.Expires);
        var tokenMappedToUser = _idGenerator.NewGuid;
        await _dbService.StoreTemporaryTokenToUser(userInfo, tokenMappedToUser, expiration);

        var claims = new[]
        {
            new Claim("UserGuid", tokenMappedToUser.ToString()),
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

    public async Task<AuthenticatedUser> DecodeToken(string token)
    {
        var data = _securityTokenHandler.ReadToken(token);

        var tokenExpiryDate = data.ValidTo;
        if (tokenExpiryDate < _dateTimeProvider.Now)
        {
            throw new InvalidDataException("Token has expired!");
        }

        var userGuid = ((JwtSecurityToken)data).Claims.First(claim => claim.Type == "UserGuid");
        var userInfoFromDb = await _dbService.GetUserFromToken(Guid.Parse(userGuid.Value));
        if (userInfoFromDb == null)
        {
            throw new InvalidDataException("Token is not valid");
        }

        if (userInfoFromDb.Expires < _dateTimeProvider.Now)
        {
            throw new InvalidDataException("Invalid token: expired");
        }

        return new AuthenticatedUser(userInfoFromDb.UserId);
    }
}
