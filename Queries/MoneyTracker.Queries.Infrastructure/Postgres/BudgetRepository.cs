using System.Data;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Entities.BudgetCategory;
using MoneyTracker.Queries.Domain.Repositories;
using Npgsql;

namespace MoneyTracker.Queries.Infrastructure.Postgres;
public class BudgetRepository : IBudgetRepository
{
    private readonly IDatabase _database;
    public BudgetRepository(IDatabase db)
    {
        _database = db;
    }

    public async Task<List<BudgetGroupEntity>> GetBudget()
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

        Dictionary<int, BudgetGroupEntity> res = [];
        while (await reader.ReadAsync())
        {
            var budgetId = reader.GetInt32("id");
            if (!res.TryGetValue(budgetId, out BudgetGroupEntity? group))
            {
                group = new BudgetGroupEntity(reader.GetString("name"));
                res.Add(budgetId, group);
            }

            if (!reader.IsDBNull("category_name"))
            {
                decimal planned = reader.GetDecimal("planned");
                decimal actual = reader.GetDecimal("actual");
                res[budgetId].AddBudgetCategory(new BudgetCategoryEntity(
                    reader.GetString("category_name"),
                    planned,
                    actual,
                    planned - actual)
                );
            }
        }

        return res.Values.ToList();
    }
}
