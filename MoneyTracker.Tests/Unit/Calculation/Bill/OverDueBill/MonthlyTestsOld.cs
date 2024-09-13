
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class MonthlyTestsOld
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Monthly().MatchCommand("Monthly"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_SameDay_Null()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(24, new DateOnly(2024, 8, 24), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_BeforeNextDueDate_Null()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(30, new DateOnly(2024, 8, 30), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDate_ReturnOneIteration()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        var fiveDaysBeforeIteration = month.CalculateOverDueBill(19, new DateOnly(2024, 8, 19), dateProvider);
        Assert.Equal(new OverDueBillInfo(5, 1), fiveDaysBeforeIteration);

        var tenDaysBeforeIteration = month.CalculateOverDueBill(14, new DateOnly(2024, 8, 14), dateProvider);
        Assert.Equal(new OverDueBillInfo(10, 1), tenDaysBeforeIteration);

        var fifteenDaysBeforeIteration = month.CalculateOverDueBill(9, new DateOnly(2024, 8, 9), dateProvider);
        Assert.Equal(new OverDueBillInfo(15, 1), fifteenDaysBeforeIteration);

        var twentyThreeDaysBeforeIteration = month.CalculateOverDueBill(1, new DateOnly(2024, 8, 1), dateProvider);
        Assert.Equal(new OverDueBillInfo(23, 1), twentyThreeDaysBeforeIteration);
    }

    //[Fact]
    public void CalculateOverDueBillInfo_OnTheDayOfTheSecondIteration_ReturnsOneIteration()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        var twentyThreeDaysBeforeIteration = month.CalculateOverDueBill(24, new DateOnly(2024, 6, 24), dateProvider);
        Assert.Equal(new OverDueBillInfo(61, 1), twentyThreeDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_TwoIterationAfterNextDueDate_ReturnTwoIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(24, new DateOnly(2024, 7, 24), dateProvider);
        Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);

        var fourtyOneDaysBeforeIteration = month.CalculateOverDueBill(14, new DateOnly(2024, 7, 14), dateProvider);
        Assert.Equal(new OverDueBillInfo(41, 2), fourtyOneDaysBeforeIteration);

        var fiftyOneDaysBeforeIteration = month.CalculateOverDueBill(4, new DateOnly(2024, 7, 4), dateProvider);
        Assert.Equal(new OverDueBillInfo(51, 2), fiftyOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf31EndOfMonth_ReturnXIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 7, 31));

        var month = new Monthly();

        var thrityDaysBeforeIteration = month.CalculateOverDueBill(1, new DateOnly(2024, 7, 1), dateProvider);
        Assert.Equal(new OverDueBillInfo(30, 1), thrityDaysBeforeIteration);

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(31, new DateOnly(2024, 6, 30), dateProvider);
        Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIteration30To30MonthDay30_ReturnXIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 7, 30));

        var month = new Monthly();

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 6, 30), dateProvider);
        Assert.Equal(new OverDueBillInfo(30, 1), thrityOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf30EndOfMonth_ReturnXIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 6, 30));

        var month = new Monthly();

        var twentyNineDaysBeforeIteration = month.CalculateOverDueBill(1, new DateOnly(2024, 6, 1), dateProvider);
        Assert.Equal(new OverDueBillInfo(29, 1), twentyNineDaysBeforeIteration);

        //var thrityDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 5, 31), dateProvider);
        //Assert.Equal(new OverDueBillInfo(30, 1), thrityDaysBeforeIteration);

        //var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(new DateOnly(2024, 5, 30), dateProvider);
        //Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);

        var thrityTwoDaysBeforeIteration = month.CalculateOverDueBill(29, new DateOnly(2024, 5, 29), dateProvider);
        Assert.Equal(new OverDueBillInfo(32, 2), thrityTwoDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf1st_ReturnXIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 6, 1));

        var month = new Monthly();

        var oneDayBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(31, new DateOnly(2024, 5, 31), dateProvider);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeIterationButOneMonthDiff);

        var twoDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(30, new DateOnly(2024, 5, 30), dateProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), twoDaysBeforeIterationButOneMonthDiff);

        var threeDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(29, new DateOnly(2024, 5, 29), dateProvider);
        Assert.Equal(new OverDueBillInfo(3, 1), threeDaysBeforeIterationButOneMonthDiff);
    }

    [Fact]
    public void CalculateOverDueBillInfo_OneIterationAfterNextDueDateMonthOf1stOf30th_ReturnXIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 7, 1));

        var month = new Monthly();

        var oneDayBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(30, new DateOnly(2024, 6, 30), dateProvider);
        Assert.Equal(new OverDueBillInfo(1, 1), oneDayBeforeIterationButOneMonthDiff);

        var rwoDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(29, new DateOnly(2024, 6, 29), dateProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), rwoDaysBeforeIterationButOneMonthDiff);

        var threeDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(29, new DateOnly(2024, 6, 29), dateProvider);
        Assert.Equal(new OverDueBillInfo(2, 1), threeDaysBeforeIterationButOneMonthDiff);
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrDateDayNumberLessThanNextDueDateDayNumber()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 10, 4));

        var month = new Monthly();

        var nextDueDate = month.CalculateOverDueBill(30, new DateOnly(2024, 8, 30), dateProvider);
        Assert.Equal(new OverDueBillInfo(35, 2), nextDueDate);
    }
}
