namespace MoneyTracker.Shared.Models.Budget
{
    public class BudgetCategoryDTO : IBudgetDTO
    {
        public string Name { get; set; }
        public decimal Planned { get; set; }
        public decimal Actual { get; set; }
        public decimal Difference
        {
            get
            {
                return _difference;
            }
            protected set
            {
                _difference = Planned - Actual;
            }
        }

        private decimal _difference;
    }
}
