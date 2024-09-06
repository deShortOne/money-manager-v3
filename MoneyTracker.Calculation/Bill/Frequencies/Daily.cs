
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Daily : IFrequency
{
    public OverDueBillInfo? CalculateOverDueBill(DateOnly nextDueDate, IDateTimeProvider dateTimeProvider)
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.Now);
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }

        return new OverDueBillInfo(numberOfDaysOverdue, numberOfDaysOverdue);
    }
    public bool MatchCommand(string frequency) => frequency.Equals("Daily");
}
