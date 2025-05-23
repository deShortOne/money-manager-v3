using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public class CalculateNationalInsurance : IWageCalculator
{
    private readonly IWageCalculator _next;
    public static readonly Percentage UkNationalInsuranceTax = Percentage.From(8);

    public CalculateNationalInsurance(IWageCalculator next)
    {
        _next = next;
    }

    public PreTaxGrossIncomeResult CalculatePreTaxGrossIncome(Money grossYearlyWage)
    {
        return _next.CalculatePreTaxGrossIncome(grossYearlyWage);
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        var result = _next.CalculateYearlyWage(grossYearlyWage);

        return result with
        {
            NationalInsurance = result.TaxableIncome * UkNationalInsuranceTax,
        };
    }
}
