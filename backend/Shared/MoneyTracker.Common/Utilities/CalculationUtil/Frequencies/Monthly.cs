using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
internal class Monthly : IFrequency
{
    private string _name = "Monthly";

    public string GetName() => _name;
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate)
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

    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateTimeProvider dateProvider)
    {
        var today = DateOnly.FromDateTime(dateProvider.Now);
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }

        return new OverDueBillInfo(numberOfDaysOverdue, GetOverDueDatesLis(nextDueDate, today, monthDay));
    }

    public bool MatchCommand(string frequency) => frequency == _name;

    public static DateOnly[] GetOverDueDatesLis(DateOnly date1, DateOnly date2, int monthDay)
    {
        List<DateOnly> previousDates = [];
        while (date1 < date2)
        {
            previousDates.Add(date1);
            date1 = date1.AddMonths(1);

            var maxDayOfNewMonth = new DateOnly(date1.Year, date1.Month, 1).AddMonths(1).AddDays(-1);
            var setDayInMonth = Math.Min(monthDay, maxDayOfNewMonth.Day);
            date1 = new DateOnly(date1.Year, date1.Month, setDayInMonth);
        }

        return [.. previousDates];
    }
}
