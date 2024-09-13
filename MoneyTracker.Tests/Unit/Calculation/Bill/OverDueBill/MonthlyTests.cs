
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
}
