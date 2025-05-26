using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode.GivenTaxCodeL.GivenThePersonalAllowanceIsLowerThanTheWage;
public sealed class WhenTheTheTaxableIncomeIsWithinTheAdditionalRateRange
{
    private WageResult _subject;

    public WhenTheTheTaxableIncomeIsWithinTheAdditionalRateRange()
    {
        var personalAllowance = Money.From(10000);

        var subject = new CalculateProgressiveTaxAfterPersonalAllowance(personalAllowance);

        _subject = subject.CalculateYearlyWage(Money.From(150000));
    }

    [Fact]
    public void ThenTheTaxableIncomeIsCorrect()
    {
        Assert.Equal(Money.From(140000), _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIsCorrect()
    {
        Assert.Equal(Money.From(49831.5m), _subject.TotalTaxPayable);
    }
}
