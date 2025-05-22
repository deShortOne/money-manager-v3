using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage.Pension;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAPensionCalculation;
public sealed class WhenTheAmountIsRequested
{
    private Money _actual;

    public WhenTheAmountIsRequested()
    {
        var next = new Mock<WageInterface>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var pensionRequested = new FixedPensionAmount(Money.From(200));

        var subject = new CalculatePension(next.Object, pensionRequested);

        _actual = subject.CalculateYearlyWage(Money.From(12000)).Pension;
    }

    [Fact]
    public void ThenThePensionAmountIsTakenOff()
    {
        Assert.Equal(Money.From(2400), _actual);
    }
}
