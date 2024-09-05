
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Daily : IFrequency
{
    public OverDueBillInfo? Calculate(DateOnly nextDueDate, IDateTimeProvider dateTimeProvider)
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.Now);
        if (today == nextDueDate)
        {
            return null;
        }

        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;
        return new OverDueBillInfo(numberOfDaysOverdue, numberOfDaysOverdue);
    }
    public bool MatchCommand(string frequency) => frequency.Equals("Daily");
}
