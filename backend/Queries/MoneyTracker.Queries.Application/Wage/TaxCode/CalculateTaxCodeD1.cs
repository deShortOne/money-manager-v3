using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeD1 : IWageCalculator
{
    public static readonly Percentage TaxRate = Percentage.From(45);

    private readonly CalculateTaxAsAllIncomeAtRate _actual;

    public CalculateTaxCodeD1()
    {
        _actual = new CalculateTaxAsAllIncomeAtRate(TaxRate);
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return _actual.CalculateYearlyWage(grossYearlyWage);
    }
}
