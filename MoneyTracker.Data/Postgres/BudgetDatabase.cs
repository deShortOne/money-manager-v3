using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.RepositoryToService.Budget;
using MoneyTracker.Shared.Models.ServiceToRepository.Budget;
using Npgsql;

namespace MoneyTracker.Data.Postgres;

public class BudgetDatabase : IBudgetDatabase
{
    private readonly IDatabase _database;
    public BudgetDatabase(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<BudgetGroupEntityDTO>> GetBudget()
    {
        string query = """
            SELECT bg.id,
            	bg."name",
            	c.name AS category_name,
            	COALESCE(category_sum.amount, 0) AS actual,
            	bc.planned
            FROM budgetcategory bc
            LEFT JOIN (
            	SELECT category_id,
            		sum(amount) AS amount
            	FROM register
                WHERE account_id IN (
                    SELECT id
                    FROM account
                    WHERE users_id = 1
                )
            	GROUP BY category_id
            	) category_sum
            	ON category_sum.category_id = bc.category_id
            INNER JOIN category c
            	ON bc.category_id = c.id
            RIGHT JOIN budgetgroup bg
            	ON bg.id = bc.budget_group_id
            ORDER BY bg.id,
            	bg."name",
            	c.name;
            """;

        using var reader = await _database.GetTable(query);

        Dictionary<int, BudgetGroupEntityDTO> res = [];
        while (await reader.ReadAsync())
        {
            var budgetId = reader.GetInt32("id");
            if (!res.TryGetValue(budgetId, out BudgetGroupEntityDTO? group))
            {
                group = new BudgetGroupEntityDTO(reader.GetString("name"));
                res.Add(budgetId, group);
            }

            if (!reader.IsDBNull("category_name"))
            {
                decimal planned = reader.GetDecimal("planned");
                decimal actual = reader.GetDecimal("actual");
                res[budgetId].AddBudgetCategoryDTO(new BudgetCategoryEntityDTO(
                    reader.GetString("category_name"),
                    planned,
                    actual,
                    planned - actual)
                );
            }
        }

        return res.Values.ToList();
    }

    public async Task<BudgetCategoryEntityDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget)
    {
        // TODO - USERS ID
        var queryInsertIntoBudgetCategory = """
            INSERT INTO budgetcategory VALUES
                (1, @budgetGroupId, @categoryId, @planned)
            ON CONFLICT (users_id, budget_group_id, category_id) DO UPDATE
                SET planned = @planned
            RETURNING (SELECT name FROM category WHERE id = @categoryId),
                (planned),
                (coalesce((SELECT SUM(amount) actual FROM register WHERE category_id = @categoryId), 0)) actual;
            """;
        var queryInsertIntoBudgetCategoryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("budgetGroupId", newBudget.BudgetGroupId),
            new NpgsqlParameter("planned", newBudget.Planned),
            new NpgsqlParameter("categoryId", newBudget.CategoryId),
        };

        var reader = await _database.GetTable(queryInsertIntoBudgetCategory, queryInsertIntoBudgetCategoryParams);
        if (await reader.ReadAsync())
        {
            var name = reader.GetString("name");
            var planned = reader.GetDecimal("planned");
            var actual = reader.GetDecimal("actual");
            return new BudgetCategoryEntityDTO(name, planned, actual, planned - actual);
        }
        throw new ExternalException("Database failed to return data");
    }

    public async Task<List<BudgetGroupEntityDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCateogry)
    {
        var setParamsLis = new List<string>();
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", editBudgetCateogry.BudgetCategoryId),
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
                WHERE category_id = @id;
                """;
        var reader = await _database.UpdateTable(query, queryParams);
        if (reader != 1)
        {
            throw new Exception("Unknown error");
        }

        return await GetBudget();
    }

    public async Task<bool> DeleteBudgetCategory(DeleteBudgetCategoryDTO deleteBudgetCategory)
    {
        var query = """
            DELETE FROM budgetcategory
            WHERE category_id = @id;
            """;
        var queryParams = new List<DbParameter>()
        {
            new NpgsqlParameter("id", deleteBudgetCategory.BudgetCategoryId),
        };
        var reader = await _database.UpdateTable(query, queryParams);
        return reader == 1;
    }
}
