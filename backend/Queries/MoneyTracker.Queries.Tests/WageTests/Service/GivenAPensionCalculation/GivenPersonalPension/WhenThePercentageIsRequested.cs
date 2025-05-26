using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAPensionCalculation.GivenPersonalPension;
public sealed class WhenThePercentageIsRequested
{
    private readonly CalculatePension _subject;

    public WhenThePercentageIsRequested()
    {
        var next = new Mock<IWageCalculator>();
        next
            .Setup(x => x.CalculatePreTaxGrossIncome(It.IsAny<Money>()))
            .Returns(new PreTaxGrossIncomeResult());
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var pensionRequested = new PercentagePensionAmount(Percentage.From(10));
        var pension = new Pension(pensionRequested, PensionType.Personal);

        _subject = new CalculatePension(next.Object, pension);
    }

    [Fact]
    public void ThenThePensionAmountITakenOffWhenCalculatingPreTax()
    {
        var actual = _subject.CalculatePreTaxGrossIncome(Money.From(12000)).Pension;
        Assert.Equal(Money.From(1200), actual);
    }

    [Fact]
    public void ThenThePensionAmountIsNotTakenOffWhenCalculatingYearlyWage()
    {
        var actual = _subject.CalculateYearlyWage(Money.From(12000)).Pension;
        Assert.Null(actual);
    }
}
