
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class BiWeeklyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new BiWeekly().MatchCommand("BiWeekly"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneIterationBefore_Null()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new BiWeekly();

        Assert.Null(week.CalculateOverDueBill(new DateOnly(2024, 8, 26), currentDay));
        Assert.Null(week.CalculateOverDueBill(new DateOnly(2024, 8, 29), currentDay));
        Assert.Null(week.CalculateOverDueBill(new DateOnly(2024, 9, 2), currentDay));
        Assert.Null(week.CalculateOverDueBill(new DateOnly(2024, 9, 6), currentDay));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekAfter_ReturnsOneIterationLate()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new BiWeekly();

        var oneDayBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 8, 23), currentDay);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeResult);

        var threeDaysBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 8, 15), currentDay);
        Assert.Equal(new OverDueBillInfo(9, 1), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 8, 11), currentDay);
        Assert.Equal(new OverDueBillInfo(13, 1), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksAfter_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new BiWeekly();

        var oneDayBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 8, 10), currentDay);
        Assert.Equal(new OverDueBillInfo(14, 2), oneDayBeforeResult);

        var threeDaysBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 8, 3), currentDay);
        Assert.Equal(new OverDueBillInfo(21, 2), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.CalculateOverDueBill(new DateOnly(2024, 7, 28), currentDay);
        Assert.Equal(new OverDueBillInfo(27, 2), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksBefore_Null()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new BiWeekly();

        Assert.Null(week.CalculateOverDueBill(new DateOnly(2024, 9, 12), currentDay));
    }
}
