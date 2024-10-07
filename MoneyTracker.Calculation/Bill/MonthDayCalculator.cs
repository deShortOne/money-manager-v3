
namespace MoneyTracker.Calculation.Bill;
public class MonthDayCalculator
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
