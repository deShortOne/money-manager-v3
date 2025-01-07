using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Common.Utilities.CalculationUtil;
public interface IFrequencyCalculation
{
    DateOnly CalculateNextDueDate(string frequency, int monthDay, DateOnly currDueDate);
    OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate);
    bool DoesFrequencyExist(string frequency);
    List<string> GetFrequencyNames();
}

public class FrequencyCalculation : IFrequencyCalculation
{
    private readonly IDateTimeProvider _dateProvider;
    private readonly IEnumerable<IFrequency> _frequencies;

    public FrequencyCalculation(IDateTimeProvider dateProvider) : this(dateProvider, new Daily(), new Weekly(), new BiWeekly(), new Monthly())
    {
    }

    public FrequencyCalculation(IDateTimeProvider dateProvider, params IFrequency[] frequencies)
    {
        _dateProvider = dateProvider;
        _frequencies = frequencies;
    }

    public OverDueBillInfo? CalculateOverDueBillInfo(int monthDay, string frequency, DateOnly nextDueDate)
    {
        foreach (var f in _frequencies)
        {
            if (f.MatchCommand(frequency))
            {
                return f.CalculateOverDueBill(monthDay, nextDueDate, _dateProvider);
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

    public List<string> GetFrequencyNames()
    {
        var res = new List<string>();
        foreach (var f in _frequencies)
            res.Add(f.GetName());
        return res;
    }
}
