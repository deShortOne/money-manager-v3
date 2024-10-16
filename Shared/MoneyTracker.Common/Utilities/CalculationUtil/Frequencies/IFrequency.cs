using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
public interface IFrequency
{
    public bool MatchCommand(string frequency);
    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateTimeProvider dateProvider);
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate);
}
