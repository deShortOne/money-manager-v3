using MoneyTracker.Common.DTOs;
using MoneyTracker.Common.Utilities.CalculationUtil.Frequencies;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Tests.FrequencyCalculationTests.Local;

namespace MoneyTracker.Tests.FrequencyCalculationTests.OverDueBill;
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
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2024, 8, 24));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 24), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayBeforeNextDueDate_Null()
    {
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2024, 8, 24));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 25), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2024, 8, 24));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 23), dateProvider);

        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2024, 8, 23)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2024, 8, 24));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 22), dateProvider);

        Assert.Equal(new OverDueBillInfo(2, [new DateOnly(2024, 8, 22), new DateOnly(2024, 8, 23)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_LeapYearCurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2024, 3, 1));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 2, 28), dateProvider);

        Assert.Equal(new OverDueBillInfo(2, [new DateOnly(2024, 2, 28), new DateOnly(2024, 2, 29)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_NotLeapYearCurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateTimeProvider dateProvider = TestHelper.CreateMockdateProvider(new DateTime(2023, 3, 1));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2023, 2, 28), dateProvider);

        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2023, 2, 28)]), overdueBillInfo);
    }
}
