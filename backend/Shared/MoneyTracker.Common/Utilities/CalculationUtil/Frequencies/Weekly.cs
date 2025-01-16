using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
internal class Weekly : IFrequency
{
    private string _name = "Weekly";

    public string GetName() => _name;
    public DateOnly CalculateNextDueDate(int monthDay, DateOnly currNextDueDate)
    {
        return currNextDueDate.AddDays(7);
    }

    public OverDueBillInfo? CalculateOverDueBill(int monthDay, DateOnly nextDueDate, IDateTimeProvider dateProvider)
    {
        var today = DateOnly.FromDateTime(dateProvider.Now);
        int numberOfDaysOverdue = today.DayNumber - nextDueDate.DayNumber;

        if (numberOfDaysOverdue <= 0)
        {
            return null;
        }

        return new OverDueBillInfo(numberOfDaysOverdue, GetOverDueDatesLis(nextDueDate, today));
    }

    public bool MatchCommand(string frequency) => frequency == _name;

    public static DateOnly[] GetOverDueDatesLis(DateOnly date1, DateOnly date2)
    {
        List<DateOnly> previousDates = [];
        while (date1 < date2)
        {
            previousDates.Add(date1);
            date1 = date1.AddDays(7);
        }

        return [.. previousDates];
    }
}
