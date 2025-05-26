using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenTaxCodeL.GivenThePersonalAllowanceIsLowerThanTheWage;
public sealed class WhenTheTheTaxableIncomeIsWithinThePersonalAllowanceRange
{
    private WageResult _subject;

    public WhenTheTheTaxableIncomeIsWithinThePersonalAllowanceRange()
    {
        var personalAllowance = Money.From(10000);

        var subject = new CalculateProgressiveTaxAfterPersonalAllowance(personalAllowance);

        _subject = subject.CalculateYearlyWage(Money.From(12000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIsCorrect()
    {
        Assert.Equal(Money.From(2000), _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIs0()
    {
        Assert.Equal(Money.From(400), _subject.TotalTaxPayable);
    }
}
