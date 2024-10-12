using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;

namespace MoneyTracker.API;

public class Startup
{
    public static void SetBillDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillRepository, BillRepository>();
    }

    public static void SetBudgetDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetRepository, BudgetRepository>();
    }

    public static void SetCategoryDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryRepository, CategoryRepository>();
    }

    public static void SetRegisterDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterRepository, RegisterRepository>();
    }

    public static void SetupAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Money Tracker",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])
                        ),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>()
            .AddSingleton<IPasswordHasher, PasswordHasher>();
    }
}
