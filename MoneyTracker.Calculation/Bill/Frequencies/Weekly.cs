
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Weekly : IFrequency
{
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate)
    {
        return currNextDueDate.AddDays(7);
    }

    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateProvider dateProvider)
    {
        var today = dateProvider.Now;
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }
        return new OverDueBillInfo(numberOfDaysOverdue, numberOfDaysOverdue / 7 + 1);
    }

    public bool MatchCommand(string frequency) => frequency == "Weekly";
}
