using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAPensionCalculation.GivenAutoEnrolmentPension;
public sealed class WhenTheAmountIsRequested
{
    private readonly CalculatePension _subject;

    public WhenTheAmountIsRequested()
    {
        var next = new Mock<IWageCalculator>();
        next
            .Setup(x => x.CalculatePreTaxGrossIncome(It.IsAny<Money>()))
            .Returns(new PreTaxGrossIncomeResult());
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var pensionRequested = new FixedPensionAmount(Money.From(200));
        var pension = new Pension(pensionRequested, PensionType.AutoEnrolment);

        _subject = new CalculatePension(next.Object, pension);
    }

    [Fact]
    public void ThenThePensionAmountIsNotTakenOffWhenCalculatingPreTax()
    {
        var actual = _subject.CalculatePreTaxGrossIncome(Money.From(12000)).Pension;
        Assert.Null(actual);
    }

    [Fact]
    public void ThenThePensionAmountIsTakenOffWhenCalculatingYearlyWage()
    {
        var actual = _subject.CalculateYearlyWage(Money.From(12000)).Pension;
        Assert.Equal(Money.From(2400), actual);
    }
}

