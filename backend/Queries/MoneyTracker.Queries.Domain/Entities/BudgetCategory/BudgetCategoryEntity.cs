
namespace MoneyTracker.Queries.Domain.Entities.BudgetCategory;
public class BudgetCategoryEntity(string name, decimal planned, decimal actual, decimal difference)
{
    public string Name { get; } = name;
    public decimal Planned { get; } = planned;
    public decimal Actual { get; } = actual;
    public decimal Difference { get; } = difference;

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetCategoryEntity;
        if (other == null)
        {
            return false;
        }

        return Name == other.Name && Planned == other.Planned &&
            Actual == other.Actual && Difference == other.Difference;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Planned, Actual, Difference);
    }
}
