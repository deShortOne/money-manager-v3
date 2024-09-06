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

    public static OverDueBillInfo? CalculateOverDueBillInfo(DateOnly nextDueDate, string frequency, IDateTimeProvider dateTimeProvider)
    {
        foreach (var f in Frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateOverDueBill(nextDueDate, dateTimeProvider);
            }
        }
        throw new NotImplementedException("Frequency type \"{frequency}\" not found");
    }
}
