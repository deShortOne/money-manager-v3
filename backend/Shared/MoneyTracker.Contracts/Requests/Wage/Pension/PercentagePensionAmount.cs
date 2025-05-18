using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.Pension;
public class PercentagePensionAmount(Percentage percentage) : IPension
{
    public Money CalculatePension(Money salary) => salary * percentage;
}
