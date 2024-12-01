using System.Data;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.Category;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.CategoryTests.Repository;
public class CategoryRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public ICategoryCommandRepository _categoryRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _categoryRepo = new CategoryCommandRepository(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    protected async Task<List<CategoryEntity>> GetAllCategoryEntities()
    {
        var getBudgetQuery = @"
                            SELECT id, name
                            FROM category;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBudgetInfo = new NpgsqlCommand(getBudgetQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBudgetInfo.ExecuteReader();
        List<CategoryEntity> results = [];
        while (reader.Read())
        {
            results.Add(new CategoryEntity(
                id: reader.GetInt32("id"),
                name: reader.GetString("name")
            ));
        }
        return results;
    }
}
