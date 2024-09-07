namespace MoneyTracker.Shared.Models.Budget
{
    public class EditBudgetCategoryDTO
    {
        public EditBudgetCategoryDTO(int budgetGroupId, int categoryId, decimal planned)
        {
            BudgetGroupId = budgetGroupId;
            CategoryId = categoryId;
            Planned = planned;
        }

        public int BudgetGroupId { get; private set; }
        public int CategoryId { get; private set; }
        public decimal Planned { get; private set; }
    }
}
