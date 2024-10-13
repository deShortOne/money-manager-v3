
namespace MoneyTracker.Common.Utilities.CalculationUtil;
public interface IMonthDayCalculator
{
    int Calculate(DateOnly date);
}

public class MonthDayCalculator : IMonthDayCalculator
{
    public int Calculate(DateOnly date)
    {
        var nextDate = date.AddDays(1);
        if (date.Month != nextDate.Month)
        {
            return 31;
        }
        return date.Day;
    }
}
