using System.Data;
using System.Data.Common;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Budget;
using Npgsql;

namespace MoneyTracker.Data.Postgres
{
    public class BudgetDatabase : IBudgetDatabase
    {
        private readonly PostgresDatabase _database;
        public BudgetDatabase(IDatabase db)
        {
            _database = (PostgresDatabase)db;
        }

        public async Task<List<BudgetGroupDTO>> GetBudget()
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

            Dictionary<int, BudgetGroupDTO> res = [];
            while (await reader.ReadAsync())
            {
                var budgetId = reader.GetInt32("id");
                if (!res.TryGetValue(budgetId, out BudgetGroupDTO group))
                {
                    group = new BudgetGroupDTO(reader.GetString("name"));
                    res.Add(budgetId, group);
                }

                if (!reader.IsDBNull("category_name"))
                {
                    res[budgetId].AddBudgetCategoryDTO(new BudgetCategoryDTO(
                        reader.GetString("category_name"),
                        reader.GetDecimal("planned"),
                        reader.GetDecimal("actual")));
                }
            }

            return res.Values.ToList();
        }

        public async Task<BudgetCategoryDTO> AddBudgetCategory(NewBudgetCategoryDTO newBudget)
        {
            var queryInsertIntoBudgetCategory = """
                INSERT INTO budgetcategory VALUES
                    (@budgetGroupId, @categoryId, @planned)
                ON CONFLICT (budget_group_id, category_id) DO UPDATE
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
                return new BudgetCategoryDTO(name, planned, actual);
            }

            return null;
        }

        public async Task<List<BudgetGroupDTO>> EditBudgetCategory(EditBudgetCategoryDTO editBudgetCateogry)
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

        public async Task<bool> DeleteBudgetCategory(DeleteBudgetCategory deleteBudgetCategory)
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
}
