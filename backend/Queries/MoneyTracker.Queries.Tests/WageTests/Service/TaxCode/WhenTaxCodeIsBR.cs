using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;

namespace MoneyTracker.Queries.Tests.WageTests.Service.TaxCode;
public sealed class WhenTaxCodeIsBR
{
    private WageResult _subject;
    private Money _yearlyGrossIncome = Money.From(12000);

    public WhenTaxCodeIsBR()
    {
        var subject = new CalculateTaxCodeBR();

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
        Assert.Equal(Money.From(2400), _subject.TotalTaxPayable);
    }
}
