
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.NextDueDate;
public sealed class BiWeeklyTests
{

    [Fact]
    public void SimpleGetNextDueDate()
    {
        var timeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 08, 24, 0, 0, 0));

        var week = new BiWeekly();

        var nextDueDate = week.CalculateNextDueDate(-1, new DateOnly(2024, 08, 24), timeProvider);
        Assert.Equal(new DateOnly(2024, 9, 7), nextDueDate);
    }
}
