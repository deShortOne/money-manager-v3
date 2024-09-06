
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.NextDueDate;
public sealed class DailyTests
{
    [Fact]
    public void SimpleGetNextDueDate()
    {
        var timeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 08, 24, 0, 0, 0));
        Assert.Equal(new DateOnly(2024, 08, 25), new Daily().CalculateNextDueDate(-1, new DateOnly(2024, 08, 24), timeProvider));
    }
}
