
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
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
    public void CalculateOverDueBillInfo_OnTheDay_Null()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 24), currentDay));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneIterationBefore_Null()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 26), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 29), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 9, 2), currentDay));
        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 9, 6), currentDay));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekAfter_ReturnsOneIterationLate()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        var oneDayBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 23), currentDay);
        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2024, 8, 23)]), oneDayBeforeResult);

        var threeDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 15), currentDay);
        Assert.Equal(new OverDueBillInfo(9, [new DateOnly(2024, 8, 15)]), threeDaysBeforeResult);

        var sixDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 11), currentDay);
        Assert.Equal(new OverDueBillInfo(13, [new DateOnly(2024, 8, 11)]), sixDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OnTheDayOfTheSecondWeek_ReturnsOneIterationLate()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        var fourteenDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 10), currentDay);
        Assert.Equal(new OverDueBillInfo(14, [new DateOnly(2024, 8, 10)]), fourteenDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksAfter_ReturnsTwoIterationsLate()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        var twentyOneDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 8, 3), currentDay);
        Assert.Equal(new OverDueBillInfo(21, [new DateOnly(2024, 8, 3), new DateOnly(2024, 8, 17)]), twentyOneDaysBeforeResult);

        var twentySevenDaysBeforeResult = week.CalculateOverDueBill(-1, new DateOnly(2024, 7, 28), currentDay);
        Assert.Equal(new OverDueBillInfo(27, [new DateOnly(2024, 7, 28), new DateOnly(2024, 8, 11)]), twentySevenDaysBeforeResult);
    }

    [Fact]
    public void CalculateOverDueBillInfo_BetweenOneAndTwoWeeksBefore_Null()
    {
        IDateProvider currentDay = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var week = new BiWeekly();

        Assert.Null(week.CalculateOverDueBill(-1, new DateOnly(2024, 9, 12), currentDay));
    }
}
