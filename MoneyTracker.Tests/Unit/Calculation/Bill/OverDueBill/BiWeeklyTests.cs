
using MoneyTracker.Calculation.Bill.Frequencies;

namespace MoneyTracker.Tests.Unit.Calculation.Bill.OverDueBill;
public sealed class BiWeeklyTests
{
    [Fact]
    public void CalculateOverDueBillInfo_NameMatch_True()
    {
        Assert.True(new BiWeekly().MatchCommand("BiWeekly"));
    }
}
