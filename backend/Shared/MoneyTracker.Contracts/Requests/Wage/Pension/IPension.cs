using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.Pension;
public interface IPension
{
    public Money CalculatePension(Money salary);
}
