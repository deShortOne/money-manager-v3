namespace MoneyTracker.Shared.Models.Budget
{
    public interface IBudget
    {
        string Name { get; }
        decimal Planned { get; }
        decimal Actual { get; }
        decimal Difference { get; }
    }
}
