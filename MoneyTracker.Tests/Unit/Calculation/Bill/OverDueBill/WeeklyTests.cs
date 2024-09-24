
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
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
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new Weekly();

        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 24), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 26), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 28), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 30), currentDay));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekAfter_ReturnsOneIterationLate()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new Weekly();

        var oneDayBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 23), currentDay);
        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2024, 8, 23)]), oneDayBeforeResult);

        var threeDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 21), currentDay);
        Assert.Equal(new OverDueBillInfo(3, [new DateOnly(2024, 8, 21)]), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 18), currentDay);
        Assert.Equal(new OverDueBillInfo(6, [new DateOnly(2024, 8, 18)]), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksAfter_ReturnsTwoIterationsLate()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new Weekly();

        var oneDayBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 16), currentDay);
        Assert.Equal(new OverDueBillInfo(8, [new DateOnly(2024, 8, 16), new DateOnly(2024, 8, 23)]), oneDayBeforeResult);

        var threeDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 14), currentDay);
        Assert.Equal(new OverDueBillInfo(10, [new DateOnly(2024, 8, 14), new DateOnly(2024, 8, 21)]), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 11), currentDay);
        Assert.Equal(new OverDueBillInfo(13, [new DateOnly(2024, 8, 11), new DateOnly(2024, 8, 18)]), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksBefore_Null()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new Weekly();

        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 9, 5), currentDay));
    }
}
