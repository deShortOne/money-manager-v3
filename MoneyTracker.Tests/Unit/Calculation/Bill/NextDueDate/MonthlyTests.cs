
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.NextDueDate;
public sealed class MonthlyTests
{
    [Fact]
    public void GetNextDueDate()
    {
        var timeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 08, 24, 0, 0, 0));

        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(24, new DateOnly(2024, 8, 24), timeProvider);
        Assert.Equal(new DateOnly(2024, 9, 24), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_31stTo31st()
    {
        var timeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 08, 24, 0, 0, 0));

        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(31, new DateOnly(2024, 7, 31), timeProvider);
        Assert.Equal(new DateOnly(2024, 8, 31), nextDueDate);
    }

    [Fact]
    public void GetNextDueDate_31stTo30th()
    {
        var timeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 08, 24, 0, 0, 0));

        var month = new Monthly();

        var nextDueDate = month.CalculateNextDueDate(31, new DateOnly(2024, 5, 31), timeProvider);
        Assert.Equal(new DateOnly(2024, 6, 30), nextDueDate);
    }
}
