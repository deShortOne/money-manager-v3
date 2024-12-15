using MoneyTracker.Queries.DatabaseMigration;
using MoneyTracker.Queries.DatabaseMigration.Models;
using MoneyTracker.Queries.Domain.Entities.Category;
using MoneyTracker.Queries.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.CategoryTests.Repository;
public sealed class GetAllCategoriesTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));

        return;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async void FirstLoadCheckTablesThatDataAreThere()
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var budgetDb = new CategoryRepository(db);

        var actual = await budgetDb.GetAllCategories();

        var expected = new List<CategoryEntity>()
        {
            new(2, "Bills : Cell Phone"),
            new(3, "Bills : Rent"),
            new(4, "Groceries"),
            new(5, "Hobby"),
            new(6, "Pet Care"),
            new(1, "Wages & Salary : Net Pay"),
        };

        Assert.Equal(expected, actual);
    }
}
