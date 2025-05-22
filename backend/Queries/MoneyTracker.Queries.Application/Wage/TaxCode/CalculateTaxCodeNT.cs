using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeNT : IWageCalculator
{
    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return new WageResult
        {
            TaxableIncome = Money.Zero,
            TotalTaxPayable = Money.Zero,
        };
    }
}
