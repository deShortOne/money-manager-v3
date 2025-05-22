using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public interface IWageCalculator
{
    public WageResult CalculateYearlyWage(Money grossYearlyWage);
}
