using System.Data.Common;
using MoneyTracker.Commands.Domain.Entities.BudgetCategory;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using Npgsql;

namespace MoneyTracker.Commands.Infrastructure.Postgres;
public class BudgetCommandRepository : IBudgetCommandRepository
{
    private readonly IDatabase _database;
    public BudgetCommandRepository(IDatabase db)
    {
        _database = db;
    }
    public async Task AddBudgetCategory(BudgetCategoryEntity newBudgetCategory)
    {
        var queryInsertIntoBudgetCategory = """
            INSERT INTO budgetcategory VALUES
                (@userId, @budgetGroupId, @categoryId, @planned)
            ON CONFLICT (users_id, budget_group_id, category_id) DO UPDATE
                SET planned = @planned;
            """;
        var queryInsertIntoBudgetCategoryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("userId", newBudgetCategory.UserId),
            new NpgsqlParameter("budgetGroupId", newBudgetCategory.BudgetGroupId),
            new NpgsqlParameter("planned", newBudgetCategory.Planned),
            new NpgsqlParameter("categoryId", newBudgetCategory.CategoryId),
        };

        await _database.GetTable(queryInsertIntoBudgetCategory, queryInsertIntoBudgetCategoryParams);
    }

    public async Task EditBudgetCategory(EditBudgetCategoryEntity editBudgetCateogry)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", editBudgetCateogry.BudgetCategoryId),
            new NpgsqlParameter("user_id", editBudgetCateogry.UserId),
        };
        if (editBudgetCateogry.BudgetCategoryPlanned != null)
        {
            setParamsLis.Add("planned = @planned");
            queryParams.Add(new NpgsqlParameter("planned", editBudgetCateogry.BudgetCategoryPlanned));
        }
        if (editBudgetCateogry.BudgetGroupId != null)
        {
            setParamsLis.Add("budget_group_id = @budget_group_id");
            queryParams.Add(new NpgsqlParameter("budget_group_id", editBudgetCateogry.BudgetGroupId));
        }

        var query = $"""
            UPDATE budgetcategory
            SET {string.Join(",", setParamsLis)}
            WHERE category_id = @id
                AND users_id = @user_id;
            """;
        var reader = await _database.UpdateTable(query, queryParams);
        if (reader != 1)
        {
            throw new InvalidDataException("Unknown error");
        }
    }

    public async Task DeleteBudgetCategory(DeleteBudgetCategoryEntity deleteBudgetCategory)
    {
        var query = """
            DELETE FROM budgetcategory
            WHERE category_id = @id
                AND users_id = @user_id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", deleteBudgetCategory.BudgetCategoryId),
            new NpgsqlParameter("user_id", deleteBudgetCategory.UserId),
        };
        await _database.UpdateTable(query, queryParams);
    }
}
