
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class DailyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Daily().MatchCommand("Daily"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_SameDay_Null()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(new DateOnly(2024, 8, 24), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayBeforeNextDueDate_Null()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(new DateOnly(2024, 8, 25), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(new DateOnly(2024, 8, 23), dateTimeProvider);

        Assert.Equal(new OverDueBillInfo(1, 1), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(new DateOnly(2024, 8, 22), dateTimeProvider);

        Assert.Equal(new OverDueBillInfo(2, 2), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_LeapYearCurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 3, 1, 0, 0, 0));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(new DateOnly(2024, 2, 28), dateTimeProvider);

        Assert.Equal(new OverDueBillInfo(2, 2), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_NotLeapYearCurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2023, 3, 1, 0, 0, 0));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(new DateOnly(2023, 2, 28), dateTimeProvider);

        Assert.Equal(new OverDueBillInfo(1, 1), overdueBillInfo);
    }
}
