namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetCategoryDTO
    {
        public string Name { get; set; }
        public decimal Planned { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference => Planned - Actual;
    }
}
