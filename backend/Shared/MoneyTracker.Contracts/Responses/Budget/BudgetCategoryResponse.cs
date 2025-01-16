
namespace MoneyTracker.Contracts.Responses.Budget;
public class BudgetCategoryResponse(int id, string name, decimal planned, decimal actual, decimal difference)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public decimal Planned { get; } = planned;
    public decimal Actual { get; } = actual;
    public decimal Difference { get; } = difference;

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetCategoryResponse;

        if (other == null)
        {
            return false;
        }

        return Id == other.Id
            && Name == other.Name
            && Planned == other.Planned
            && Actual == other.Actual
            && Difference == other.Difference;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Planned, Actual, Difference);
    }
}
