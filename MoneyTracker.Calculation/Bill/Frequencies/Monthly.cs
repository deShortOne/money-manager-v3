
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill.Frequencies;
internal class Monthly : IFrequency
{
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate, IDateTimeProvider dateTimeProvider)
    {
        var tmpNextDueDate = currNextDueDate.AddMonths(1);
        if (tmpNextDueDate.Day < monthDay)
        {
            if (tmpNextDueDate.Month == tmpNextDueDate.AddDays(1).Month)
            {
                tmpNextDueDate = tmpNextDueDate.AddDays(1);
            }
        }
        return tmpNextDueDate;
    }

    public OverDueBillInfo? CalculateOverDueBill(DateOnly nextDueDate, IDateTimeProvider dateTimeProvider)
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.Now);
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }

        int numberOfMonthsDifference;
        if (today.Month == nextDueDate.Month)
        {
            numberOfMonthsDifference = 1;
        }
        else
        {
            numberOfMonthsDifference = today.Month - nextDueDate.Month + 1;
            if (nextDueDate.AddMonths(1) >= today)
            {
                numberOfMonthsDifference--;
            }
        }

        return new OverDueBillInfo(numberOfDaysOverdue, numberOfMonthsDifference);
    }

    public bool MatchCommand(string frequency) => frequency == "Monthly";
}
