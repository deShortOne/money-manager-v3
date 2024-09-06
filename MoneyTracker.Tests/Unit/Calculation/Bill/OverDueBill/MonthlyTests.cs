
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class MonthlyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Monthly().MatchCommand("Monthly"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_SameDay_Null()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(new DateOnly(2024, 8, 24), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_BeforeNextDueDate_Null()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(new DateOnly(2024, 8, 30), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDate_ReturnOneIteration()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        var fiveDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 8, 19), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(5, 1), fiveDaysBeforeIteration);

        var tenDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 8, 14), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(10, 1), tenDaysBeforeIteration);

        var fifteenDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 8, 9), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(15, 1), fifteenDaysBeforeIteration);

        var twentyThreeDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 8, 1), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(23, 1), twentyThreeDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_TwoIterationAfterNextDueDate_ReturnTwoIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 7, 24), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);

        var fourtyOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 7, 14), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(41, 2), fourtyOneDaysBeforeIteration);

        var fiftyOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 7, 4), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(51, 2), fiftyOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf31EndOfMonth_ReturnXIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 7, 31, 0, 0, 0));

        var month = new Monthly();

        var thrityDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 7, 1), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(30, 1), thrityDaysBeforeIteration);

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 6, 30), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(31, 2), thrityOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf30EndOfMonth_ReturnXIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 6, 30, 0, 0, 0));

        var month = new Monthly();

        var twentyNineDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 6, 1), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(29, 1), twentyNineDaysBeforeIteration);

        var thrityDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 5, 31), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(30, 1), thrityDaysBeforeIteration);

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 5, 30), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);

        var thrityTwoDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 5, 29), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(32, 2), thrityTwoDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf1st_ReturnXIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 6, 1, 0, 0, 0));

        var month = new Monthly();

        var oneDayBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 5, 31), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeIterationButOneMonthDiff);

        var twoDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 5, 30), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), twoDaysBeforeIterationButOneMonthDiff);

        var threeDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 5, 29), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(3, 1), threeDaysBeforeIterationButOneMonthDiff);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf1stOf30th_ReturnXIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 7, 1, 0, 0, 0));

        var month = new Monthly();

        var oneDayBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 6, 30), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeIterationButOneMonthDiff);

        var rwoDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 6, 29), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), rwoDaysBeforeIterationButOneMonthDiff);

        var threeDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(new DateOnly(2024, 6, 29), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), threeDaysBeforeIterationButOneMonthDiff);
    }
}
