using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenAMoneyObjectIsMultiplied
{
    public void ThenTheCorrectResultIsReturned()
    {
        var subject = Money.From(23);
        Assert.Equal(Money.From(46), subject * 2);
    }
}
