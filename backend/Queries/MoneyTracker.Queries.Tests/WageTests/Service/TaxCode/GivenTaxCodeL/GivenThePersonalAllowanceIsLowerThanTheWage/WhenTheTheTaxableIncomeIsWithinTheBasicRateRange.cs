using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenTaxCodeL.GivenThePersonalAllowanceIsLowerThanTheWage;
public sealed class WhenTheTheTaxableIncomeIsWithinTheBasicRateRange
{
    private WageResult _subject;

    public WhenTheTheTaxableIncomeIsWithinTheBasicRateRange()
    {
        var personalAllowance = Money.From(10000);

        var subject = new TaxCodeL(personalAllowance);

        _subject = subject.CalculateYearlyWage(Money.From(30000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIsCorrect()
    {
        Assert.Equal(Money.From(20000), _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIsCorrect()
    {
        Assert.Equal(Money.From(4000), _subject.TotalTaxPayable);
    }
}
