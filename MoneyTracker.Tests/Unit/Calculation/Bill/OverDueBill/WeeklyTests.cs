
using MoneyTracker.Calculation.Bill.Frequencies;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class WeeklyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new Weekly().MatchCommand("Weekly"));
    }
}
