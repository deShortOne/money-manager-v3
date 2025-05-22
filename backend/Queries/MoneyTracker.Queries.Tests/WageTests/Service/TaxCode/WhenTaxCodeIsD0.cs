using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode;
public sealed class WhenTaxCodeIsD0
{
    private WageResult _subject;
    private Money _yearlyGrossIncome = Money.From(12000);

    public WhenTaxCodeIsD0()
    {
        var subject = new CalculateTaxCodeD0();

        _subject = subject.CalculateYearlyWage(_yearlyGrossIncome);
    }

    [Fact]
    public void ThenTheTaxableIncomeIsTheSameAsGrossIncome()
    {
        Assert.Equal(_yearlyGrossIncome, _subject.TaxableIncome);
    }

    [Fact]
    public void ThenTheTotalTaxPayableIsCorrect()
    {
        Assert.Equal(Money.From(4800), _subject.TotalTaxPayable);
    }
}
