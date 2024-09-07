
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Tests.Database.Postgres.TestModels;

public class TestBudgetCategoryDTO
{
    public string Name { get; set; }
    public decimal Planned { get; set; }
    public decimal Actual { get; set; }
    public decimal Difference { get; set; }

    // Not TestBudgetCategoryDTO!!!
    public override bool Equals(object? obj)
    {
        var other = obj as BudgetCategoryDTO;

        if (other == null)
        {
            return false;
        }

        return Name == other.Name && Planned == other.Planned &&
            Actual == other.Actual && Difference == other.Difference;
    }

    public override int GetHashCode()
    {
        return (from c in Name
                select (int)c).Sum();
    }
}
