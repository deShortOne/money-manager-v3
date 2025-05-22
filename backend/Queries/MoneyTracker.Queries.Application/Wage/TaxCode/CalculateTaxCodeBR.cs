using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeBR : IWageCalculator
{
    public static readonly Percentage TaxRate = Percentage.From(20);

    private readonly CalculateTaxAsAllIncomeAtRate _actual;

    public CalculateTaxCodeBR()
    {
        _actual = new CalculateTaxAsAllIncomeAtRate(TaxRate);
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return _actual.CalculateYearlyWage(grossYearlyWage);
    }
}
