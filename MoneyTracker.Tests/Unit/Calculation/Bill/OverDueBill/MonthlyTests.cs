
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

        Assert.Null(month.Calculate(new DateOnly(2024, 8, 24), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_BeforeNextDueDate_Null()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        Assert.Null(month.Calculate(new DateOnly(2024, 8, 30), dateTimeProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDate_ReturnOneIteration()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        var fiveDaysBeforeIteration = month.Calculate(new DateOnly(2024, 8, 19), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(5, 1), fiveDaysBeforeIteration);

        var tenDaysBeforeIteration = month.Calculate(new DateOnly(2024, 8, 14), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(10, 1), tenDaysBeforeIteration);

        var fifteenDaysBeforeIteration = month.Calculate(new DateOnly(2024, 8, 9), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(15, 1), fifteenDaysBeforeIteration);

        var twentyThreeDaysBeforeIteration = month.Calculate(new DateOnly(2024, 8, 1), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(23, 1), twentyThreeDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_TwoIterationAfterNextDueDate_ReturnTwoIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var month = new Monthly();

        var thrityOneDaysBeforeIteration = month.Calculate(new DateOnly(2024, 7, 24), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(31, 2), thrityOneDaysBeforeIteration);

        var fourtyOneDaysBeforeIteration = month.Calculate(new DateOnly(2024, 7, 14), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(41, 2), fourtyOneDaysBeforeIteration);

        var fiftyOneDaysBeforeIteration = month.Calculate(new DateOnly(2024, 7, 4), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(51, 2), fiftyOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf31EndOfMonth_ReturnXIterations()
    {
        IDateTimeProvider dateTimeProvider = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 7, 31, 0, 0, 0));

        var month = new Monthly();

        var thrityDaysBeforeIteration = month.Calculate(new DateOnly(2024, 7, 1), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(30, 1), thrityDaysBeforeIteration);

        var thrityOneDaysBeforeIteration = month.Calculate(new DateOnly(2024, 6, 30), dateTimeProvider);
        Assert.Equal(new OverDueBillInfo(31, 2), thrityOneDaysBeforeIteration);
    }
}
