using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Calculation.Bill;

public class BillCalculation
{
    private static readonly List<IFrequency> Frequencies = [];
    public static OverDueBillInfo? CalculateOverDueBillInfo(DateOnly nextDueDate, string frequency)
    {
        foreach (var f in Frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.Calculate(nextDueDate);
            }
        }
        throw new NotImplementedException("Frequency type \"{frequency}\" not found");
    }
}
