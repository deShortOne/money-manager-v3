
using MoneyTracker.Calculation.Bill.Frequencies;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Tests.Local;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class WeeklyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Weekly().MatchCommand("Weekly"));
    }

    [Fact]
    public void CalculateOverDueBillInfo_WithinOneWeekBefore_Null()
    {
        IDateTimeProvider currentDay = TestHelper.CreateMockDateTimeProvider(new DateTime(2024, 8, 24, 0, 0, 0));

        var week = new Weekly();

        Assert.Null(week.Calculate(new DateOnly(2024, 8, 24), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 26), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 28), currentDay));
        Assert.Null(week.Calculate(new DateOnly(2024, 8, 30), currentDay));
    }
}
