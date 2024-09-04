
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Daily : IFrequency
{
    public OverDueBillInfo? Calculate(DateOnly nextDueDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        if (today == nextDueDate)
        {
            return null;
        }

        int numberOfDaysOverdue = nextDueDate.CompareTo(today);
        return new OverDueBillInfo(numberOfDaysOverdue, numberOfDaysOverdue);
    }
    public bool MatchCommand(string frequency) => frequency.Equals("Daily");
}
