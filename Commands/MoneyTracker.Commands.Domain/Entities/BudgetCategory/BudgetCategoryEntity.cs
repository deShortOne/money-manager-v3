
namespace MoneyTracker.Commands.Domain.Entities.BudgetCategory;
public class BudgetCategoryEntity(int budgetGroupId, int categoryId, decimal planned)
{
    public int BudgetGroupId { get; } = budgetGroupId;
    public int CategoryId { get; } = categoryId;
    public decimal Planned { get; } = planned;

    public override bool Equals(object? obj)
    {
        var other = obj as BudgetCategoryEntity;
        if (other == null) return false;
        return BudgetGroupId == other.BudgetGroupId &&
            CategoryId == other.CategoryId &&
            Planned == other.Planned;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BudgetGroupId, CategoryId, Planned);
    }
}
