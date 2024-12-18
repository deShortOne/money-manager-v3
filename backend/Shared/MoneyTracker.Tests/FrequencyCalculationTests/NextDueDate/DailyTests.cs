using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;

namespace MoneyTracker.Tests.FrequencyCalculationTests.NextDueDate;
public sealed class DailyTests
{
    [Fact]
    public void SimpleGetNextDueDate()
    {
        Assert.Equal(new DateOnly(2024, 08, 25), new Daily().CalculateNextDueDate(-1, new DateOnly(2024, 08, 24)));
    }
}
