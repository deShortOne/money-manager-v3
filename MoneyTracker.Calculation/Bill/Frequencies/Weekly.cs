
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

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

        return new OverDueBillInfo(numberOfDaysOverdue, GetOverDueDatesLis(nextDueDate, today));
    }

    public bool MatchCommand(string frequency) => frequency == "Weekly";

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
