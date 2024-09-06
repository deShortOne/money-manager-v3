
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
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
}
