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
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;
using MoneyTracker.Queries.Infrastructure.Mongo;
using MoneyTracker.Queries.Infrastructure.Postgres;
using MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;

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

        var database = new PostgresDatabase(builder.Configuration["Database:Paelagus_RO"]!);
        var cache = new MongoDatabase(builder.Configuration["Datastore:Cache"]!);

        builder.Services.AddSingleton(_ => cache);
        Startup.Start(builder);
        PlatformServiceStartup.StartSubscriber(builder);

        builder.Services
            .AddHttpContextAccessor()
            .AddSingleton<IDatabase>(_ => database)
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserDatabase, UserDatabase>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IUserRepositoryService, UserRepository>();

        builder.Services
            .AddSingleton<IAccountService, AccountService>()
            .AddSingleton<IAccountDatabase, AccountDatabase>()
            .AddSingleton<IAccountRepositoryService, AccountRepository>()
            .AddSingleton<IAccountCache, AccountCache>();

        builder.Services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillDatabase, BillDatabase>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IFrequencyCalculation, FrequencyCalculation>()
            .AddSingleton<IBillRepositoryService, BillRepository>()
            .AddSingleton<IBillCache, BillCache>();

        builder.Services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetDatabase, BudgetDatabase>()
            .AddSingleton<IBudgetRepositoryService, BudgetRepository>()
            .AddSingleton<IBudgetCache, BudgetCache>();

        builder.Services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryDatabase, CategoryDatabase>()
            .AddSingleton<ICategoryRepositoryService, CategoryRepository>()
            .AddSingleton<ICategoryCache, CategoryCache>();

        builder.Services
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterDatabase, RegisterDatabase>()
            .AddSingleton<IRegisterRepositoryService, RegisterRepository>()
            .AddSingleton<IRegisterCache, RegisterCache>();

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
}
