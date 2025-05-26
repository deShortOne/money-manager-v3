using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenAPercentageIsMultiplied
{
    [Fact]
    public void ThenTheCorrectValueIsReturned()
    {
        var money = Money.From(100);
        var percentage = Percentage.From(50);

        Assert.Equal(Money.From(50), money * percentage);
    }
}
