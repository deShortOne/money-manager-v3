
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Tests.Database.Postgres.TestModels;

public class TestBudgetCategoryDTO : BudgetCategoryDTO
{
    public new decimal Difference { get; set; }
}
