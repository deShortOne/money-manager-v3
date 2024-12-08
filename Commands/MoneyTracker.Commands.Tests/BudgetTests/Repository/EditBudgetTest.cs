using MoneyTracker.Commands.DatabaseMigration;
using MoneyTracker.Commands.DatabaseMigration.Models;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using Npgsql;

namespace MoneyTracker.Commands.Tests.BudgetTests.Repository;
public sealed class EditBudgetTest : BudgetRespositoryTestHelper
{
    private readonly int _userId = 1;
    private readonly int _budgetGroupId = 3;
    private readonly decimal _planned = 90;
    private readonly int _categoryId = 1;

    private async Task SetupDb()
    {
        Migration.CheckMigration(_postgres.GetConnectionString(), new MigrationOption(true));
        var deleteAllDataFromBillTable = "DELETE FROM budgetcategory;";
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await using var commandDeleteAllBudgetData = new NpgsqlCommand(deleteAllDataFromBillTable, conn);
        await conn.OpenAsync();
        await commandDeleteAllBudgetData.ExecuteNonQueryAsync();

        var addBaseBillData = """
            INSERT INTO budgetcategory (users_id, budget_group_id, planned, category_id) VALUES (@user_id, @budget_group, @amount, @categoryId);
            """;
        await using var commandAddBaseBillData = new NpgsqlCommand(addBaseBillData, conn);
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@user_id", _userId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@budget_group", _budgetGroupId));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@amount", _planned));
        commandAddBaseBillData.Parameters.Add(new NpgsqlParameter("@categoryId", _categoryId));
        await commandAddBaseBillData.ExecuteNonQueryAsync();
    }

    public static TheoryData<int?, int?> OnlyOneItemNotNull = new() {
        { 5, null },
        { null, 2 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditBaseBillItemInDatabase(int? budgetGroupId, int? planned)
    {
        await SetupDb();

        var editBillRequest = new EditBudgetCategoryEntity(_userId, _categoryId, budgetGroupId, planned);
        await _budgetRepo.EditBudgetCategory(editBillRequest);

        var expectedBillEntity = new BudgetCategoryEntity(_userId,
            budgetGroupId ?? _budgetGroupId,
            _categoryId,
            planned ?? _planned);

        var results = await GetAllBudgetCategoryEntities();

        Assert.Multiple(() =>
        {
            Assert.Single(results);
            Assert.Equal(expectedBillEntity, results[0]);
        });
    }
}
