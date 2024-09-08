
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal interface IFrequency
{
    public bool MatchCommand(string frequency);
    public OverDueBillInfo? CalculateOverDueBill(DateOnly nextDueDate, IDateProvider dateProvider);
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate);
}
