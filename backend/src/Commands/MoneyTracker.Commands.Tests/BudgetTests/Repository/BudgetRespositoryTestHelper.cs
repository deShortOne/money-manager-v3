using System.Data;
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Commands.Infrastructure.Postgres;
using Npgsql;
using Testcontainers.PostgreSql;

namespace MoneyTracker.Commands.Tests.BudgetTests.Repository;
public class BudgetRespositoryTestHelper : IAsyncLifetime
{
    public readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
#if RUN_LOCAL
        .WithDockerEndpoint("tcp://localhost:2375")
#endif
        .WithImage("postgres:16")
        .WithCleanUp(true)
        .Build();

    public IBudgetCommandRepository _budgetRepo;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption());

        var _database = new PostgresDatabase(_postgres.GetConnectionString());
        _budgetRepo = new BudgetCommandRepository(_database);
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }

    protected async Task<List<BudgetCategoryEntity>> GetAllBudgetCategoryEntities()
    {
        var getBudgetQuery = @"
                            SELECT users_id, budget_group_id, planned, category_id
                            FROM budgetcategory;
                            ";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandGetBudgetInfo = new NpgsqlCommand(getBudgetQuery, conn);
        await conn.OpenAsync();
        using var reader = commandGetBudgetInfo.ExecuteReader();
        List<BudgetCategoryEntity> results = [];
        while (reader.Read())
        {
            results.Add(new BudgetCategoryEntity(userId: reader.GetInt32("users_id"),
                budgetGroupId: reader.GetInt32("budget_group_id"),
                planned: reader.GetDecimal("planned"),
                categoryId: reader.GetInt32("category_id")
            ));
        }
        return results;
    }
}
