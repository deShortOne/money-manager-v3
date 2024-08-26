namespace MoneyTracker.API.Models
{
    public class BudgetCategory : IBudget
    {
        public string Name { get; set; }
        public decimal Planned { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference => Planned - Actual;
    }
}
