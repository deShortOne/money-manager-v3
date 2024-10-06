using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.Calculation.Bill;

public interface IFrequencyCalculation
{
    DateOnly CalculateNextDueDate(string frequency, int monthDay, DateOnly currDueDate);
    OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate, IDateProvider dateProvider);
}

public class FrequencyCalculation : IFrequencyCalculation
{
    private readonly IEnumerable<IFrequency> _frequencies;

    public FrequencyCalculation() : this(new Daily(), new Weekly(), new BiWeekly(), new Monthly())
    {
    }

    public FrequencyCalculation(params IFrequency[] frequencies)
    {
        _frequencies = frequencies;
    }

    public OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate, IDateProvider dateProvider)
    {
        foreach (var f in _frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateOverDueBill(monthDay, nextDueDate, dateProvider);
            }
        }
        throw new NotImplementedException($"Frequency type \"{frequency}\" not found");
    }

    public DateOnly CalculateNextDueDate(string frequency, int monthDay, DateOnly currDueDate)
    {
        foreach (var f in _frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateNextDueDate(monthDay, currDueDate);
            }
        }
        throw new NotImplementedException("Frequency type \"{frequency}\" not found");
    }
}
