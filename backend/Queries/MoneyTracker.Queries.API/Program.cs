using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using MoneyTracker.Authentication;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.PlatformService;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Infrastructure.Postgres;

[ExcludeFromCodeCoverage]
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
            config.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date"
            });
        });

        var databaseConnectionString = GetCliArgumentValue<string>(args, "--database") ?? builder.Configuration["Database:Paelagus_RO"]!;
        if (databaseConnectionString == null || databaseConnectionString == "")
        {
            throw new Exception("Database connection string must be set under --database");
        }

        var cacheConnectionString = GetCliArgumentValue<string>(args, "--cache") ?? builder.Configuration["Database:Cache"]!;
        var doesCacheConnectionStringExist = cacheConnectionString != null && cacheConnectionString != "";

        var useMessaging = DoesCliArgumentExist(args, "--use-messaging");
        if (useMessaging)
        {
            var rabbitConnectionString = GetCliArgumentValue<string>(args, "--rabbit") ?? builder.Configuration["Messaging:Lepus"]!;
            if (rabbitConnectionString != null && rabbitConnectionString != "")
            {
                PlatformServiceStartup.StartSubscriber(builder, rabbitConnectionString);
            }
            else
            {
                throw new Exception("Messaging connection string must be set under --rabbit");
            }

            if (!doesCacheConnectionStringExist)
            {
                throw new Exception("Cache must be set if messaging is set using --cache");
            }
        }

        builder.Services
            .AddSingleton<IDatabase>(_ => new PostgresDatabase(databaseConnectionString));
        SetupBaseApplication(builder);

        if (doesCacheConnectionStringExist)
        {
            builder.Services.AddSingleton(_ => new MongoDatabase(cacheConnectionString!));
            SetupCacheAside(builder);
        }
        else
        {
            SetupDatabaseOnly(builder);
        }

        Startup.Start(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void SetupBaseApplication(WebApplicationBuilder builder)
    {
        builder.Services
            .AddHttpContextAccessor()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserDatabase, UserDatabase>()
            .AddSingleton<IAuthenticationService, AuthenticationService>();

        builder.Services
            .AddSingleton<IAccountService, AccountService>()
            .AddSingleton<IAccountDatabase, AccountDatabase>();

        builder.Services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillDatabase, BillDatabase>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IFrequencyCalculation, FrequencyCalculation>();

        builder.Services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetDatabase, BudgetDatabase>();

        builder.Services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryDatabase, CategoryDatabase>();

        builder.Services
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterDatabase, RegisterDatabase>();

        builder.Services
            .AddSingleton<IWageService, WageService>();
    }

    private static void SetupCacheAside(WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IUserRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.UserRepository>();

        builder.Services
            .AddSingleton<IAccountRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.AccountRepository>()
            .AddSingleton<IAccountCache, AccountCache>();

        builder.Services
            .AddSingleton<IBillRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.BillRepository>()
            .AddSingleton<IBillCache, BillCache>();

        builder.Services
            .AddSingleton<IBudgetRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.BudgetRepository>()
            .AddSingleton<IBudgetCache, BudgetCache>();

        builder.Services
            .AddSingleton<ICategoryRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.CategoryRepository>()
            .AddSingleton<ICategoryCache, CategoryCache>();

        builder.Services
            .AddSingleton<IRegisterRepositoryService, MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern.RegisterRepository>()
            .AddSingleton<IRegisterCache, RegisterCache>();
    }

    private static void SetupDatabaseOnly(WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IUserRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.UserRepository>();

        builder.Services
            .AddSingleton<IAccountRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.AccountRepository>();

        builder.Services
            .AddSingleton<IBillRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.BillRepository>();

        builder.Services
            .AddSingleton<IBudgetRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.BudgetRepository>();

        builder.Services
            .AddSingleton<ICategoryRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.CategoryRepository>();

        builder.Services
            .AddSingleton<IRegisterRepositoryService, MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly.RegisterRepository>();
    }

    private static T? GetCliArgumentValue<T>(string[] args, string key) where T : class
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (args[index] == key)
            {
                return (T)Convert.ChangeType(args[index + 1], typeof(T));
            }
        }

        return null;
    }

    private static bool DoesCliArgumentExist(string[] args, string key)
    {
        return args.Any(x => x == key);
    }
}
