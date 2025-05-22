using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenANationalInsuranceCalculation;
public sealed class WhenRequested
{
    private Money _actual;

    public WhenRequested()
    {
        var next = new Mock<WageInterface>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult
            {
                TaxableIncome = Money.From(1235),
            });

        var grossYearlyWage = Money.From(33000);

        var subject = new CalculateNationalInsurance(next.Object);

        _actual = subject.CalculateYearlyWage(grossYearlyWage).NationalInsurance;
    }

    [Fact]
    public void ThenTheNationalInsuranceAmountIsCorrect()
    {
        Assert.Equal(Money.From(98.8m), _actual);
    }
}
