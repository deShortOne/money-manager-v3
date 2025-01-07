using MoneyTracker.Common.Utilities.CalculationUtil;
using MoneyTracker.Common.Utilities.DateTimeUtil;

namespace MoneyTracker.Tests.FrequencyCalculationTests.GetFrequencyNames;
public sealed class GetFrequencyNamesTest
{
    [Fact]
    public void Success()
    {
        var frequency = new FrequencyCalculation(new DateTimeProvider());
        Assert.Equal(["Daily", "Weekly", "BiWeekly", "Monthly"], frequency.GetFrequencyNames());
    }
}
