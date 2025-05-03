using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenAMoneyObjectIsDivided
{
    [Fact]
    public void WhenDividingByAnEvenPence()
    {
        var subject = Money.From(10.50m);
        Assert.Equal(Money.From(5.25m), subject / 2);
    }

    [Fact]
    public void WhenDividingByAnOddPence()
    {
        var subject = Money.From(5.99m);
        Assert.Equal(Money.From(2.99m), subject / 2);
    }
}
