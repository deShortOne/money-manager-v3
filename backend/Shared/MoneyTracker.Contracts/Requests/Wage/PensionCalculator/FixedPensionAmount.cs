using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
public class FixedPensionAmount(Money amount) : IPensionCalculator
{
    public Money CalculatePension(Money salary) => amount;
}
