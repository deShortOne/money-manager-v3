
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Tests.Database.Postgres.TestModels;

public class TestBudgetGroupDTO : BudgetGroupDTO
{
    public new List<TestBudgetCategoryDTO> Categories { get; set; } = [];
    public new decimal Planned { get; set; }
    public new decimal Actual { get; set; }
    public new decimal Difference { get; set; }
}

