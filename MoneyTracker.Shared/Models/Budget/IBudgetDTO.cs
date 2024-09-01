namespace MoneyTracker.Shared.Models.Budget
{
    public interface IBudgetDTO
    {
        string Name { get; }
        decimal Planned { get; }
        decimal Actual { get; }
        decimal Difference { get; }
    }
}
