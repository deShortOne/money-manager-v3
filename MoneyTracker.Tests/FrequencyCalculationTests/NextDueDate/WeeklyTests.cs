using MoneyTracker.Calculation.Bill.Frequencies;

namespace MoneyTracker.Tests.FrequencyCalculationTests.NextDueDate;
public sealed class WeeklyTests
{
    [Fact]
    public void SimpleGetNextDueDate()
    {
        var week = new Weekly();

        var nextDueDate = week.CalculateNextDueDate(-1, new DateOnly(2024, 08, 24));
        Assert.Equal(new DateOnly(2024, 08, 31), nextDueDate);
    }
}
