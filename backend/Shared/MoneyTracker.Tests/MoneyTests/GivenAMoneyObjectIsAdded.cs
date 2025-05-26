using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenAMoneyObjectIsAdded
{
    public void ThenTheCorrectResultIsReturned()
    {
        var subject1 = Money.From(23);
        var subject2 = Money.From(7);
        Assert.Equal(Money.From(30), subject1 + subject2);
    }
}
