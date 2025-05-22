using System;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage.Pension;

namespace MoneyTracker.Queries.Application.Wage;
public class CalculatePension : IWageCalculator
{
    private readonly IWageCalculator _next;
    private readonly IPension _pension;

    public CalculatePension(IWageCalculator next, IPension pension)
    {
        _next = next;
        _pension = pension;
    }
    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        var pension = _pension.CalculatePension(grossYearlyWage / 12) * 12;
        var result = _next.CalculateYearlyWage(grossYearlyWage);

        return result with
        {
            Pension = pension,
        };
    }
}
