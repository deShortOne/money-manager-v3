
using MoneyTracker.Calculation.Bill.Frequencies;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.NextDueDate;
public sealed class DailyTests
{
    [Fact]
    public void SimpleGetNextDueDate()
    {
        Assert.Equal(new DateOnly(2024, 08, 25), new Daily().CalculateNextDueDate(-1, new DateOnly(2024, 08, 24)));
    }
}
