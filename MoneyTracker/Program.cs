using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MoneyTracker.Data.Global;
using MoneyTracker.Data.Postgres;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDatabase>(_ => new PostgresDatabase(""));
builder.Services.AddTransient<IRegister, Register>()
    .AddTransient<ICategory, Category>()
    .AddTransient<IBudget, Budget>();

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
