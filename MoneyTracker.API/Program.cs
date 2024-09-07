using Microsoft.OpenApi.Models;
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;
using MoneyTracker.DatabaseMigration;
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
builder.Services.AddSingleton<IDatabase>(_ => new PostgresDatabase(dbConnString));
builder.Services.AddSingleton<IRegisterDatabase, RegisterDatabase>()
    .AddSingleton<IBillDatabase, BillDatabase>()
    .AddSingleton<ICategoryDatabase, CategoryDatabase>()
    .AddSingleton<IBudgetDatabase, BudgetDatabase>()
    .AddSingleton<IDateTimeProvider, DateTimeProvider>();

var dbResult = Migration.CheckMigration(dbConnString);
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
