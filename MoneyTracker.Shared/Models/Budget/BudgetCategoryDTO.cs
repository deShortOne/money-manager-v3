namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetCategoryDTO
    {
        public BudgetCategoryDTO(string name, decimal planned, decimal actual)
        {
            Name = name;
            Planned = planned;
            Actual = actual;
            Difference = planned - Actual;
        }

        public string Name { get; private set; }
        public decimal Planned { get; private set; }
        public decimal Actual { get; private set; }
        public decimal Difference { get; private set; }
    }
}
