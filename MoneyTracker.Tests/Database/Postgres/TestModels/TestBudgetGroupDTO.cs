
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Tests.Database.Postgres.TestModels;

public class TestBudgetGroupDTO
{
    public string Name { get; set; }
    public List<TestBudgetCategoryDTO> Categories { get; set; } = [];
    public decimal Planned { get; set; }
    public decimal Actual { get; set; }
    public decimal Difference { get; set; }

    // Not TestBudgetGroupDTO!!!
    public override bool Equals(object? obj)
    {
        var other = obj as BudgetGroupDTO;

        if (other == null)
        {
            return false;
        }

        if (Categories.Count != other.Categories.Count)
        {
            return false;
        }

        for (int i = 0; i < Categories.Count; i++)
        {
            if (!Categories[i].Equals(other.Categories[i]))
            {
                return false;
            }
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
