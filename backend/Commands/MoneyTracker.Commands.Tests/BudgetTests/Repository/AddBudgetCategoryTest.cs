
using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using Npgsql;

namespace MoneyTracker.Commands.Tests.BudgetTests.Repository;
public sealed class AddBudgetCategoryTest : BudgetRespositoryTestHelper
{
    [Fact]
    public async void AddBudgetItemIntoDatabase()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM budgetcategory;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBudgetData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBudgetData.ExecuteNonQueryAsync();

        var userId = 1;
        var budgetGroupId = 2;
        var planned = 3;
        var caregoryId = 4;
        var newBudgetEntity = new BudgetCategoryEntity(userId, budgetGroupId, planned, caregoryId);

        await _budgetRepo.AddBudgetCategory(newBudgetEntity);

        List<BudgetCategoryEntity> results = await GetAllBudgetCategoryEntities();

        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(newBudgetEntity, results[0]);
        });
    }
}
