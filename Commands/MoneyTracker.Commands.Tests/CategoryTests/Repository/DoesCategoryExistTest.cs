using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Tests.CategoryTests.Service;
public sealed class DoesCategoryExistTest : IAsyncLifetime
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

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async void ValidCategoryIdFromSeed(int categoryId)
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var category = new CategoryCommandRepository(db);

        Assert.True(await category.DoesCategoryExist(categoryId));
    }

    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(0)]
    [InlineData(-1)]
    public async void InvalidCategoryIdFromSeed(int categoryId)
    {
        var db = new PostgresDatabase(_postgres.GetConnectionString());
        var category = new CategoryCommandRepository(db);

        Assert.False(await category.DoesCategoryExist(categoryId));
    }
}
