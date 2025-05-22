using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public sealed class CalculateProgressiveTaxAfterPersonalAllowance : WageInterface
{
    private readonly Money _personalAllowance;

    public CalculateProgressiveTaxAfterPersonalAllowance(Money personalAllowance)
    {
        _personalAllowance = personalAllowance;
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        var taxableIncome = SubtractPersonalAllowance(grossYearlyWage);
        var totalTaxPayable = CalculateProgressiveTax(taxableIncome);

        return new WageResult
        {
            TaxableIncome = taxableIncome,
            TotalTaxPayable = totalTaxPayable,
        };
    }

    public Money SubtractPersonalAllowance(Money grossYearlyWage)
    {
        var taxableIncome = grossYearlyWage - _personalAllowance;
        if (taxableIncome <= Money.Zero)
        {
            taxableIncome = Money.Zero;
        }
        return taxableIncome;
    }

    public Money CalculateProgressiveTax(Money taxableIncome)
    {
        if (taxableIncome <= Money.Zero)
        {
            return Money.Zero;
        }

        var taxableIncomeRemaining = Money.From(taxableIncome);
        var totalTaxPayable = Money.Zero;
        foreach (var taxRatesAndbands in EnglandNorthernIrelandAndWalesTaxBands.TaxRatesAndBands.Skip(1))
        {
            var amountToRate = taxRatesAndbands.MaxTaxableIncome - taxRatesAndbands.MinTaxableIncome + Money.From(1);
            if (taxableIncomeRemaining <= amountToRate)
            {
                totalTaxPayable += taxableIncomeRemaining * taxRatesAndbands.Rate;
                break;
            }

            var a = amountToRate * taxRatesAndbands.Rate;
            totalTaxPayable += a;
            taxableIncomeRemaining -= amountToRate;
        }

        return totalTaxPayable;
    }
}

public class TaxRatesAndBands(string bandName, Money minTaxableIncome, Money maxTaxableIncome, Percentage rate)
{
    public string BandName { get; } = bandName;
    public Money MinTaxableIncome { get; } = minTaxableIncome;
    public Money MaxTaxableIncome { get; } = maxTaxableIncome;
    public Percentage Rate { get; } = rate;
}

public static class EnglandNorthernIrelandAndWalesTaxBands
{
    public static List<TaxRatesAndBands> TaxRatesAndBands =
    [
        new TaxRatesAndBands("Personal Allowance", Money.From(1), Money.From(12570), Percentage.From(0)), // ewww 1 as min??
        new TaxRatesAndBands("Basic Rate", Money.From(12571), Money.From(50270), Percentage.From(20)),
        new TaxRatesAndBands("Higher Rate", Money.From(50271), Money.From(125140), Percentage.From(40)),
        new TaxRatesAndBands("Additional Rate", Money.From(125141), Money.From(9999999999), Percentage.From(45)),
    ];
}
