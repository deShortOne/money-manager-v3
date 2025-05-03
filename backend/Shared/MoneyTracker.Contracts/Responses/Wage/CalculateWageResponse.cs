
namespace MoneyTracker.Contracts.Responses.Wage;
public class CalculateWageResponse
{
    public decimal GrossYearlyIncome { get; set; }
    public List<decimal> Wages { get; set; }

    public override bool Equals(object obj)
    {
        var other = obj as CalculateWageResponse;
        if (other == null) return false;
        return GrossYearlyIncome == other.GrossYearlyIncome
            && Enumerable.SequenceEqual(Wages, other.Wages);
    }
}
