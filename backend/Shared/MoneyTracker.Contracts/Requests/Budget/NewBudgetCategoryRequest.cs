
namespace MoneyTracker.Contracts.Requests.Budget;
public class NewBudgetCategoryRequest(int budgetGroupId, int categoryId, decimal planned)
{
    public int BudgetGroupId { get; } = budgetGroupId;
    public int CategoryId { get; } = categoryId;
    public decimal Planned { get; } = planned;
}
