using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.Calculation.Bill;

public class BillCalculation
{
    private static readonly List<IFrequency> Frequencies = [
        new Daily(),
        new Weekly(),
        new BiWeekly(),
        new Monthly(),
    ];

    public static OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate, IDateProvider dateProvider)
    {
        foreach (var f in Frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateOverDueBill(monthDay, nextDueDate, dateProvider);
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
