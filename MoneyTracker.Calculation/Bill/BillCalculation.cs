using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill;

public class BillCalculation
{
    private static readonly List<IFrequency> Frequencies = [
        new Daily(),
        new Weekly(),
        new BiWeekly(),
        new Monthly(),
    ];

    public static OverDueBillInfo? CalculateOverDueBillInfo(string frequency, DateOnly nextDueDate, IDateTimeProvider dateTimeProvider)
    {
        foreach (var f in Frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateOverDueBill(nextDueDate, dateTimeProvider);
            }
        }
        throw new NotImplementedException($"Frequency type \"{frequency}\" not found");
    }

    public static DateOnly CalculateNextDueDate(string frequency, int monthDay, DateOnly currDueDate)
    {
        foreach (var f in Frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateNextDueDate(monthDay, currDueDate);
            }
        }
        throw new NotImplementedException("Frequency type \"{frequency}\" not found");
    }
}
