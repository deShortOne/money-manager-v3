using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxCodeBR : WageInterface
{
    private readonly Percentage _taxRate = Percentage.From(20);
    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return new WageResult
        {
            TaxableIncome = grossYearlyWage,
            TotalTaxPayable = grossYearlyWage * _taxRate,
        };
    }
}
