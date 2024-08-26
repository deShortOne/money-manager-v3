
using MoneyTracker.API.Models;
using System.Data;

namespace MoneyTracker.API.Database
{
    public class Budget
    {
        public async Task<IEnumerable<BudgetGroup>> GetBudget()
        {
            string query = """
                SELECT bg.id,
                	bg."name",
                	c.name AS category_name,
                	category_sum.amount AS actual,
                	bc.planned
                FROM (
                	SELECT category_id,
                		sum(amount) AS amount
                	FROM bill
                	GROUP BY category_id
                	) category_sum
                INNER JOIN category c
                	ON category_sum.category_id = c.id
                INNER JOIN budgetcategory bc
                	ON category_sum.category_id = bc.category_id
                RIGHT JOIN budgetgroup bg
                	ON bg.id = bc.budget_group_id
                ORDER BY bg.id,
                	bg."name";
                """;

            using var reader = await Helper.GetTable(query);

            Dictionary<int, BudgetGroup> res = [];
            while (await reader.ReadAsync())
            {
                var budgetId = reader.GetInt32("id");
                if (!res.TryGetValue(budgetId, out BudgetGroup group))
                {
                    group = new BudgetGroup()
                    {
                        Name = reader.GetString("name"),
                    };
                    res.Add(budgetId, group);
                }


                if (!reader.IsDBNull("category_name"))
                {
                    var categoryName = reader.GetString("category_name");
                    var a = reader.GetDecimal("actual");
                    var b = reader.GetDecimal("planned");
                    res[budgetId].Categories.Add(new BudgetCategory()
                    {
                        Name = categoryName,
                        Actual = a,
                        Planned = b,
                    });
                }
            }

            return res.Values.ToList();
        }
    }
}
