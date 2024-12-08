
namespace MoneyTracker.Commands.Domain.Entities.BudgetCategory;
public class EditBudgetCategoryEntity(int userId, int budgetCategoryId, int? budgetGroupId = null, decimal? budgetCategoryPlanned = null)
{
    public int UserId {get;}  = userId;
    public int BudgetCategoryId { get; } = budgetCategoryId;
    public int? BudgetGroupId { get; } = budgetGroupId;
    public decimal? BudgetCategoryPlanned { get; } = budgetCategoryPlanned;

    public override bool Equals(object? obj)
    {
        var other = obj as EditBudgetCategoryEntity;
        if (other == null) return false;
        return BudgetCategoryId == other.BudgetCategoryId &&
            BudgetGroupId == other.BudgetGroupId &&
            BudgetCategoryPlanned == other.BudgetCategoryPlanned;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BudgetCategoryId, BudgetGroupId, BudgetCategoryPlanned);
    }
}
