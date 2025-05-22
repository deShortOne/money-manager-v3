using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage;
public interface WageInterface
{
    public WageResult CalculateYearlyWage(Money grossYearlyWage);
}
