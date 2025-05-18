using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.Pension;
public class FixedPensionAmount(Money amount) : IPension
{
    public Money CalculatePension(Money salary) => amount;
}
