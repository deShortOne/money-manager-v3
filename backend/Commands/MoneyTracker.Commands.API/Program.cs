using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using MoneyTracker.Authentication;
using MoneyTracker.Authentication.Authentication;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Application;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;

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
        Migration.CheckMigration(builder.Configuration["Database:Paelagus_RO"]!, new MigrationOption(true, true));

        Startup.Start(builder, database);
        builder.Services
            .AddSingleton<IAccountCommandRepository, AccountCommandRepository>();

        builder.Services
            .AddHttpContextAccessor()
            .AddSingleton<IDatabase>(_ => database)
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserCommandRepository, UserCommandRepository>()
            .AddSingleton<IAuthenticationService, AuthenticationService>();

        builder.Services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillCommandRepository, BillCommandRepository>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IFrequencyCalculation, FrequencyCalculation>()
            .AddSingleton<IMonthDayCalculator, MonthDayCalculator>()
            .AddSingleton<IBillCommandRepository, BillCommandRepository>();

        builder.Services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetCommandRepository, BudgetCommandRepository>();

        builder.Services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryCommandRepository, CategoryCommandRepository>();

        builder.Services
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterCommandRepository, RegisterCommandRepository>();

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
