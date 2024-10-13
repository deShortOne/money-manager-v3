using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Common.Utilities.CalculationUtil;
public interface IFrequencyCalculation
{
    DateOnly CalculateNextDueDate(string frequency, int monthDay, DateOnly currDueDate);
    OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate, IDateTimeProvider dateProvider);
    bool DoesFrequencyExist(string frequency);
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

    public OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate, IDateTimeProvider dateProvider)
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

    public bool DoesFrequencyExist(string frequency)
    {
        foreach (var f in _frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return true;
            }
        }
        return false;
    }
}
