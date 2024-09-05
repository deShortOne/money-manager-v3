
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class WeeklyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Weekly().MatchCommand("Weekly"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekBefore_Null()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new Weekly();

        Assert.Null(week.Calculate(new DateOnly(2024, 8, 24), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 26), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 28), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 30), currentDay));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekAfter_ReturnsOneIterationLate()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new Weekly();

        var oneDayBeforeResult = week.Calculate(new DateOnly(2024, 8, 23), currentDay);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeResult);

        var threeDaysBeforeResult = week.Calculate(new DateOnly(2024, 8, 21), currentDay);
        Assert.Equal(new OverDueBillInfo(3, 1), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.Calculate(new DateOnly(2024, 8, 18), currentDay);
        Assert.Equal(new OverDueBillInfo(6, 1), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksAfter_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new Weekly();

        var oneDayBeforeResult = week.Calculate(new DateOnly(2024, 8, 16), currentDay);
        Assert.Equal(new OverDueBillInfo(8, 2), oneDayBeforeResult);

        var threeDaysBeforeResult = week.Calculate(new DateOnly(2024, 8, 14), currentDay);
        Assert.Equal(new OverDueBillInfo(10, 2), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.Calculate(new DateOnly(2024, 8, 11), currentDay);
        Assert.Equal(new OverDueBillInfo(13, 2), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksBefore_Null()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new Weekly();

        Assert.Null(week.Calculate(new DateOnly(2024, 9, 5), currentDay));
    }
}
