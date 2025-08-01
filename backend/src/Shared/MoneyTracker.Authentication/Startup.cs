using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Authentication.Utils;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Authentication;
[ExcludeFromCodeCoverage]
public class Startup
{
    public static void Start(WebApplicationBuilder builder)
    {
        var issuer = builder.Configuration["Jwt:iss"]!;
        var audience = builder.Configuration["Jwt:aud"]!;
        var key = builder.Configuration["Jwt:key"]!;

        builder.Services
            .AddSingleton<IJwtConfig>(_ => new JwtConfig(issuer, audience, key, 0))
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IIdGenerator, IdGenerator>()
            .AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>();
    }
}
