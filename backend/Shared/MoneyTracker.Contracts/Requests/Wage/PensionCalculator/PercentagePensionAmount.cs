using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
public class PercentagePensionAmount(Percentage percentage) : IPensionCalculator
{
    public Money CalculatePension(Money salary) => salary * percentage;
}
