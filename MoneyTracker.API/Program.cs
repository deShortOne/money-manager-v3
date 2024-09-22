using Microsoft.OpenApi.Models;
using MoneyTracker.API;
using MoneyTracker.Core;
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
using MoneyTracker.DatabaseMigration.Models;
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
});

var dbConnString = builder.Configuration["Database:Paelagus_RO"];
builder.Services.AddSingleton<IDatabase>(_ => new PostgresDatabase(dbConnString))
    .AddSingleton<IDateProvider, DateProvider>();
builder.Services
    .AddSingleton<IUserAuthDatabase, UserAuthDatabase>()
    .AddSingleton<IUserAuthenticationService, UserAuthenticationService>()
    .AddSingleton<IJwtConfig>(_ => new JwtConfig(
        builder.Configuration["Jwt:Iss"],
        builder.Configuration["Jwt:Aud"],
        builder.Configuration["Jwt:Key"],
        int.Parse(builder.Configuration["Jwt:Expires"])
    ));

Startup.SetBillDependencyInjection(builder.Services);
Startup.SetBudgetDependencyInjection(builder.Services);
Startup.SetCategoryDependencyInjection(builder.Services);
Startup.SetRegisterDependencyInjection(builder.Services);
Startup.SetupAuthentication(builder);

var dbResult = Migration.CheckMigration(dbConnString, new MigrationOption(false));
if (!dbResult.Successful)
{
    throw new InvalidOperationException("Database failed to update, exception: " + dbResult.Error);
}

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
