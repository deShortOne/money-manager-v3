using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
public interface IPensionCalculator
{
    public Money CalculatePension(Money salary);
}
