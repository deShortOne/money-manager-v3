using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateTaxAsAllIncomeAtRate : IWageCalculator
{
    private readonly Percentage _taxRate;

    public CalculateTaxAsAllIncomeAtRate(Percentage taxRate)
    {
        _taxRate = taxRate;
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return new WageResult
        {
            TaxableIncome = grossYearlyWage,
            TotalTaxPayable = grossYearlyWage * _taxRate,
        };
    }
}
