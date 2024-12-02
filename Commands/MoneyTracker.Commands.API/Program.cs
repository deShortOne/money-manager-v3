using System.Diagnostics.CodeAnalysis;
using MoneyTracker.Authentication;
using MoneyTracker.Commands.Application;
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
        builder.Services.AddSwaggerGen();

        var database = new PostgresDatabase(builder.Configuration["Database:Paelagus_RO"]!);

        Startup.Start(builder, database);
        builder.Services
            .AddSingleton<IAccountCommandRepository, AccountCommandRepository>();

        builder.Services
            .AddHttpContextAccessor()
            .AddSingleton<IDatabase>(_ => database);

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
