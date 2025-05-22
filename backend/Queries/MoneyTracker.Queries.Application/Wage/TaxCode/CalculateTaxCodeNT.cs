using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeNT : WageInterface
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
