using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Responses.Wage;
public class CalculateWageResponse
{
    public Money GrossYearlyIncome { get; set; }
    public List<Money> Wages { get; set; }

    public override bool Equals(object obj)
    {
        var other = obj as CalculateWageResponse;
        if (other == null) return false;
        if (Wages.Count != other.Wages.Count)
            return false;
        for (int i = 0; i < Wages.Count; i++)
        {
            if (!Wages[i].Equals(other.Wages[i]))
                return false;
        }

        return GrossYearlyIncome.Equals(other.GrossYearlyIncome);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GrossYearlyIncome, Wages);
    }
}
