using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
public class FixedPensionAmount(Money amount) : IPensionCalculator
{
    public Money Amount { get; } = amount;

    public Money CalculatePension(Money salary) => Amount;
}
