
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;
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
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 24), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayBeforeNextDueDate_Null()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var day = new Daily();

        Assert.Null(day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 25), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 23), dateProvider);

        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2024, 8, 23)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 8, 22), dateProvider);

        Assert.Equal(new OverDueBillInfo(2, [new DateOnly(2024, 8, 22), new DateOnly(2024, 8, 23)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_LeapYearCurrentDayTwoDaysAfterNextDueDate_ReturnsTwoIterationsLate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 3, 1));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2024, 2, 28), dateProvider);

        Assert.Equal(new OverDueBillInfo(2, [new DateOnly(2024, 2, 28), new DateOnly(2024, 2, 29)]), overdueBillInfo);
    }

    [Fact]
    public void CalculateOverDueBillInfo_NotLeapYearCurrentDayOneDayAfterNextDueDate_ReturnsOneIterationLate()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2023, 3, 1));

        var day = new Daily();
        var overdueBillInfo = day.CalculateOverDueBill(-1, new DateOnly(2023, 2, 28), dateProvider);

        Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2023, 2, 28)]), overdueBillInfo);
    }
}
