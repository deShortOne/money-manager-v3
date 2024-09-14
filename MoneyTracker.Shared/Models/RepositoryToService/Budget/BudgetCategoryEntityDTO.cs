
namespace MoneyTracker.Shared.Models.RepositoryToService.Budget
{
    public class BudgetCategoryEntityDTO
    {
        public BudgetCategoryEntityDTO(string name, decimal planned, decimal actual, decimal difference)
        {
            Name = name;
            Planned = planned;
            Actual = actual;
            Difference = difference;
        }

        public string Name { get; private set; }
        public decimal Planned { get; private set; }
        public decimal Actual { get; private set; }
        public decimal Difference { get; private set; }

        public override bool Equals(object? obj)
        {
            var other = obj as BudgetCategoryEntityDTO;

            if (other == null)
            {
                return false;
            }

            return Name == other.Name && Planned == other.Planned &&
                Actual == other.Actual && Difference == other.Difference;
        }

        public override int GetHashCode()
        {
            return (from c in Name
                    select (int)c).Sum();
        }
    }
}
