
namespace MoneyTracker.Commands.Domain.Entities.BudgetCategory;
public class DeleteBudgetCategoryEntity(int userId, int budgetGroupId, int budgetCategoryId)
{
    public int UserId { get; } = userId;
    public int BudgetGroupId { get; } = budgetGroupId;
    public int BudgetCategoryId { get; } = budgetCategoryId;

    public override bool Equals(object? obj)
    {
        var other = obj as DeleteBudgetCategoryEntity;
        if (other == null) return false;
        return BudgetGroupId == other.BudgetGroupId &&
            BudgetCategoryId == other.BudgetCategoryId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BudgetGroupId, BudgetCategoryId);
    }
}
