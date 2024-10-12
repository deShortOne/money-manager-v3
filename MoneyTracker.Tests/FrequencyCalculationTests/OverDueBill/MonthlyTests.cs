
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.FrequencyCalculation.Tests.Local;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.FrequencyCalculation.Tests.OverDueBill;
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
        Assert.Equal(new OverDueBillInfo(31, [new DateOnly(2024, 7, 24)]), thrityOneDaysBeforeIteration);
    }

    [Fact]
    public void CalculateOverDueBillInfo_GreaterThanTwoAndLessThanThreeIterationAfterNextDueDate_ReturnTwoIterations()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 24));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var fourtyOneDaysBeforeIteration = month.CalculateOverDueBill(14, new DateOnly(2024, 7, 14), dateProvider);
            Assert.Equal(new OverDueBillInfo(41, [new DateOnly(2024, 7, 14), new DateOnly(2024, 8, 14)]), fourtyOneDaysBeforeIteration);

            var fiftyOneDaysBeforeIteration = month.CalculateOverDueBill(4, new DateOnly(2024, 7, 4), dateProvider);
            Assert.Equal(new OverDueBillInfo(51, [new DateOnly(2024, 7, 4), new DateOnly(2024, 8, 4)]), fiftyOneDaysBeforeIteration);
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
            Assert.Equal(new OverDueBillInfo(29, [new DateOnly(2024, 6, 1)]), twentyNineDaysBeforeIteration);

            var thrityTwoDaysBeforeIteration = month.CalculateOverDueBill(1, new DateOnly(2024, 5, 1), dateProvider);
            Assert.Equal(new OverDueBillInfo(60, [new DateOnly(2024, 5, 1), new DateOnly(2024, 6, 1)]), thrityTwoDaysBeforeIteration);
        });
    }


    [Fact]
    public void CalculateOverDueBillInfo_CurrDateNearStartOfMonthAndDueDateNearEnd()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 6, 1));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var oneDayBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(31, new DateOnly(2024, 5, 31), dateProvider);
            Assert.Equal(new OverDueBillInfo(1, [new DateOnly(2024, 5, 31)]), oneDayBeforeIterationButOneMonthDiff);

            var twoDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(30, new DateOnly(2024, 5, 30), dateProvider);
            Assert.Equal(new OverDueBillInfo(2, [new DateOnly(2024, 5, 30)]), twoDaysBeforeIterationButOneMonthDiff);

            var threeDaysBeforeIterationButOneMonthDiff = month.CalculateOverDueBill(29, new DateOnly(2024, 5, 29), dateProvider);
            Assert.Equal(new OverDueBillInfo(3, [new DateOnly(2024, 5, 29)]), threeDaysBeforeIterationButOneMonthDiff);

            var thirtyTwoDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 4, 30), dateProvider);
            Assert.Equal(new OverDueBillInfo(32, [new DateOnly(2024, 4, 30), new DateOnly(2024, 5, 30)]), thirtyTwoDaysBeforeIteration);
        });
    }

    [Fact]
    public void CalculateOverDueBillInfo_30To30MonthDay30()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 7, 30));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var thrityDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 6, 30), dateProvider);
            Assert.Equal(new OverDueBillInfo(30, [new DateOnly(2024, 6, 30)]), thrityDaysBeforeIteration);

            var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 5, 30), dateProvider);
            Assert.Equal(new OverDueBillInfo(61, [new DateOnly(2024, 5, 30), new DateOnly(2024, 6, 30)]), thrityOneDaysBeforeIteration);
        });
    }

    [Fact]
    public void CalculateOverDueBillInfo_31ToCurrDay30MonthDay31()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 30));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var thrityDaysBeforeIteration = month.CalculateOverDueBill(31, new DateOnly(2024, 7, 31), dateProvider);
            Assert.Equal(new OverDueBillInfo(30, [new DateOnly(2024, 7, 31)]), thrityDaysBeforeIteration);

            var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(31, new DateOnly(2024, 5, 31), dateProvider);
            Assert.Equal(new OverDueBillInfo(91, [
                new DateOnly(2024, 5, 31), new DateOnly(2024, 6, 30), new DateOnly(2024, 7, 31)
                ]), thrityOneDaysBeforeIteration);
        });
    }

    [Fact]
    public void CalculateOverDueBillInfo_30ToCurrDay31MonthDay30()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 31));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var thrityDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 7, 30), dateProvider);
            Assert.Equal(new OverDueBillInfo(32, [new DateOnly(2024, 7, 30), new DateOnly(2024, 8, 30)]), thrityDaysBeforeIteration);

            var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(30, new DateOnly(2024, 5, 30), dateProvider);
            DateOnly[] dates = [new DateOnly(2024, 5, 30), new DateOnly(2024, 6, 30), new DateOnly(2024, 7, 30),
                new DateOnly(2024, 8, 30)];
            Assert.Equal(new OverDueBillInfo(93, dates), thrityOneDaysBeforeIteration);
        });
    }

    [Fact]
    public void CalculateOverDueBillInfo_31ToCurrDay31MonthDay31()
    {
        IDateProvider dateProvider = TestHelper.CreateMockdateProvider(new DateOnly(2024, 8, 31));

        var month = new Monthly();

        Assert.Multiple(() =>
        {
            var thrityDaysBeforeIteration = month.CalculateOverDueBill(31, new DateOnly(2024, 7, 31), dateProvider);
            Assert.Equal(new OverDueBillInfo(31, [new DateOnly(2024, 7, 31)]), thrityDaysBeforeIteration);

            var thrityOneDaysBeforeIteration = month.CalculateOverDueBill(31, new DateOnly(2024, 5, 31), dateProvider);
            DateOnly[] dates = [new DateOnly(2024, 5, 31), new DateOnly(2024, 6, 30), new DateOnly(2024, 7, 31)];
            Assert.Equal(new OverDueBillInfo(92, dates), thrityOneDaysBeforeIteration);
        });
    }
}
