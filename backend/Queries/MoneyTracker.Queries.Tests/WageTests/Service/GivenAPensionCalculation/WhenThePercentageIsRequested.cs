using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage.Pension;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAPensionCalculation;
public sealed class WhenThePercentageIsRequested
{
    private Money _actual;

    public WhenThePercentageIsRequested()
    {
        var next = new Mock<WageInterface>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var pensionRequested = new PercentagePensionAmount(Percentage.From(10));

        var subject = new CalculatePension(next.Object, pensionRequested);

        _actual = subject.CalculateYearlyWage(Money.From(12000)).Pension;
    }

    [Fact]
    public void ThenThePensionAmountIsTakenOff()
    {
        Assert.Equal(Money.From(1200), _actual);
    }
}
