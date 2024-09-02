namespace MoneyTracker.Shared.Models.Budget
{
    public class DeleteBudgetCategory
    {
        public DeleteBudgetCategory(int id)
        {
            BudgetCategoryId = id;
        }

        public int BudgetCategoryId { get; private set; }
    }
}
