using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public class CalculateNationalInsurance : WageInterface
{
    private readonly WageInterface _next;
    public static readonly Percentage UkNationalInsuranceTax = Percentage.From(8);

    public CalculateNationalInsurance(WageInterface next)
    {
        _next = next;
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
