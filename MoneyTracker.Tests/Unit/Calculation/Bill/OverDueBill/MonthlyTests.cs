
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
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(24, new DateOnly(2024, 8, 24), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_BeforeDueDate_Null()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(30, new DateOnly(2024, 8, 30), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_AfterDueDateWithinOneIteration_ReturnsOneIteration()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Null(month.CalculateOverDueBill(30, new DateOnly(2024, 8, 30), dateProvider));
    }

    [Fact]
    public void CalculateOverDueBillInfo_OnTheSecondIteration_ReturnsOneIteration()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(24, new DateOnly(2024, 7, 24), dateProvider);
        Assert.Equal(new OverDueBillInfo(31, 1), thrityOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_GreaterThanTwoAndLessThanThreeIterationAfterNextDueDate_ReturnTwoIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var fourtyOneDaysBeforeIteration = month.CalculateOverDueBill(14, new DateOnly(2024, 7, 14), dateProvider);
            Assert.Equal(new OverDueBillInfo(41, 2), fourtyOneDaysBeforeIteration);

            var fiftyOneDaysBeforeIteration = month.CalculateOverDueBill(4, new DateOnly(2024, 7, 4), dateProvider);
            Assert.Equal(new OverDueBillInfo(51, 2), fiftyOneDaysBeforeIteration);
        });
    }

    [Fact]
    public void CalculateOverDueBillInfo_CurrDateNearEndOfMonthAndDueDateNearStart()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 6, 30));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var twentyNineDaysBeforeIteration = month.CalculateOverDueBill(1, new DateOnly(2024, 6, 1), dateProvider);
            Assert.Equal(new OverDueBillInfo(29, 1), twentyNineDaysBeforeIteration);

            var thrityTwoDaysBeforeIteration = month.CalculateOverDueBill(29, new DateOnly(2024, 5, 1), dateProvider);
            Assert.Equal(new OverDueBillInfo(60, 2), thrityTwoDaysBeforeIteration);
        });
    }
}
