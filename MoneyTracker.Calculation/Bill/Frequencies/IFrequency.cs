
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal interface IFrequency
{
    public bool MatchCommand(string frequency);
    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateProvider dateProvider);
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate);
}
