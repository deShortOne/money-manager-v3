using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeD0 : WageInterface
{
    public static readonly Percentage TaxRate = Percentage.From(40);

    private readonly CalculateTaxAsAllIncomeAtRate _actual;

    public CalculateTaxCodeD0()
    {
        _actual = new CalculateTaxAsAllIncomeAtRate(TaxRate);
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return _actual.CalculateYearlyWage(grossYearlyWage);
    }
}
