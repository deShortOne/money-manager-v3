using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Authentication.Utils;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Authentication;
public class Startup
{
    public static void Start(WebApplicationBuilder builder, IDatabase database)
    {
        builder.Services
            .AddSingleton(database)
            .AddSingleton<IUserAuthRepository, UserAuthRepository>()
            .AddSingleton<IJwtConfig>(_ => new JwtConfig("", "", "", 10))
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<IIdGenerator, IdGenerator>()
            .AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>()
            .AddSingleton<IUserAuthenticationService, UserAuthenticationService>();
    }
}
