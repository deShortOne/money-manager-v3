using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenTaxCodeL.GivenThePersonalAllowanceIsLowerThanTheWage;
public sealed class WhenTheTheTaxableIncomeIsWithinTheHigherRateRange
{
    private WageResult _subject;

    public WhenTheTheTaxableIncomeIsWithinTheHigherRateRange()
    {
        var personalAllowance = Money.From(10000);

        var subject = new CalculateProgressiveTaxAfterPersonalAllowance(personalAllowance);

        _subject = subject.CalculateYearlyWage(Money.From(80000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIsCorrect()
    {
        Assert.Equal(Money.From(70000), _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIsCorrect()
    {
        Assert.Equal(Money.From(20460), _subject.TotalTaxPayable);
    }
}
