using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;

namespace MoneyTracker.Tests.FrequencyCalculationTests.NextDueDate;
public sealed class BiWeeklyTests
{

    [Fact]
    public void SimpleGetNextDueDate()
    {
        var week = new BiWeekly();

        var nextDueDate = week.CalculateNextDueDate(-1, new DateOnly(2024, 08, 24));
        Assert.Equal(new DateOnly(2024, 9, 7), nextDueDate);
    }
}
