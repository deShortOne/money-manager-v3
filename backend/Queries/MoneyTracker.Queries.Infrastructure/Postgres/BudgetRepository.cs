using System.Data;
using System.Data.Common;
using MoneyTracker.Authentication.DTOs;
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

    public async Task<List<BudgetGroupEntity>> GetBudget(AuthenticatedUser user)
    {
        string query = """
            SELECT bg.id,
            	bg."name",
            	c.id AS category_id,
            	c.name AS category_name,
            	COALESCE(category_sum.amount, 0) AS actual,
            	bc.planned
            FROM (
            	SELECT category_id,
            		budget_group_id,
            		planned
            	FROM budgetcategory
            	WHERE users_id = @userId
            	) bc
            LEFT JOIN (
            	SELECT category_id,
            		SUM(amount) AS amount
            	FROM register
            	WHERE account_id IN (
            			SELECT id
            			FROM account
            			WHERE users_id = @userId
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
        var queryParams = new List<DbParameter>
        {
            new NpgsqlParameter("userId", user.Id),
        };
        using var reader = await _database.GetTable(query, queryParams);

        Dictionary<int, BudgetGroupEntity> res = [];
        foreach (DataRow row in reader.Rows)
        {
            var budgetId = row.Field<int>("id");
            if (!res.TryGetValue(budgetId, out BudgetGroupEntity? group))
            {
                group = new BudgetGroupEntity(budgetId, row.Field<string>("name")!);
                res.Add(budgetId, group);
            }

            if (row.Field<string>("category_name") != null)
            {
                decimal planned = row.Field<decimal>("planned");
                decimal actual = row.Field<decimal>("actual");
                res[budgetId].AddBudgetCategory(new BudgetCategoryEntity(
                    row.Field<int>("category_id"),
                    row.Field<string>("category_name")!,
                    planned,
                    actual,
                    planned - actual)
                );
            }
        }

        return res.Values.ToList();
    }
}
