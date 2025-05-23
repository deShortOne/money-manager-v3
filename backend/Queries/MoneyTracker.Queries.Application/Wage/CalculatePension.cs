using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;

namespace MoneyTracker.Queries.Application.Wage;
public class CalculatePension : IWageCalculator
{
    private readonly IWageCalculator _next;
    private readonly IPensionCalculator _pensionCalculator;

    public CalculatePension(IWageCalculator next, Pension pension)
    {
        _next = next;
        _pensionCalculator = pension.Calculator;
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        var pension = _pensionCalculator.CalculatePension(grossYearlyWage / 12) * 12;
        var result = _next.CalculateYearlyWage(grossYearlyWage);

        return result with
        {
            Pension = pension,
        };
    }
}
