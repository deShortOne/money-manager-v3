
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Daily : IFrequency
{
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate)
    {
        return currNextDueDate.AddDays(1);
    }

    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateProvider dateProvider)
    {
        var today = dateProvider.Now;
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }

        return new OverDueBillInfo(numberOfDaysOverdue, numberOfDaysOverdue, []);
    }
    public bool MatchCommand(string frequency) => frequency.Equals("Daily");
}
