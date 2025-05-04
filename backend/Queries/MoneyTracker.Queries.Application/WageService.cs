using System.Text.RegularExpressions;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.Application;
public class WageService : IWageService
{
    public ResultT<CalculateWageResponse> CalculateWage(CalculateWageRequest request)
    {
        var incomeFrequency = Convert(request.FrequencyOfIncome);
        if (incomeFrequency is IncomeFrequency.Unknown)
        {
            return Error.Validation("", "Invalid frequency");
        }

        var grossYearlyWage = CalculateGrossYearlyWage(request.GrossIncome, incomeFrequency);
        var response = new CalculateWageResponse
        {
            GrossYearlyIncome = grossYearlyWage,
            Wages = GetWageReducedByTax(grossYearlyWage, request.TaxCode),
        };

        return response;
    }

    private static IncomeFrequency Convert(string incomeFrequency)
    {
        Enum.TryParse(incomeFrequency, out IncomeFrequency incomeFrequencyEnum);
        return incomeFrequencyEnum;
    }

    private static Money CalculateGrossYearlyWage(decimal grossIncome, IncomeFrequency frequencyOfIncome)
    {
        return frequencyOfIncome switch
        {
            IncomeFrequency.Yearly => Money.From(grossIncome),
            IncomeFrequency.Monthly => Money.From(grossIncome * 12),
            IncomeFrequency.Every4Weeks => Money.From(grossIncome * 13),
            IncomeFrequency.Weekly => Money.From(grossIncome * 52),
            IncomeFrequency.Daily => throw new NotImplementedException(),
            // Implement number of days of week, grossIncome * numWorkDaysPerWeek * 52
            IncomeFrequency.Hourly => throw new NotImplementedException(),
            // Implement number of hours of week, grossIncome * numWorkHoursPerWeek * 52
            _ => throw new NotImplementedException(),
        };
    }

    private static List<Money> GetWageReducedByTax(Money grossYearlyWage, string taxCode)
    {
        var taxCodeElements = Regex.Match(taxCode, @"^(\d+)(\w+)$");
        var taxAmount = taxCodeElements.Groups[1].Value;
        var taxLetter = taxCodeElements.Groups[2].Value.ToUpper();

        if (taxLetter != "L")
        {
            throw new NotImplementedException("Only tax letter L is accepted");
        }

        var taxFreePersonalAllowanceYearly = Money.From(int.Parse(taxAmount) * 10);
        Money netIncomeYearly;
        Money monthlyIncome;
        if (grossYearlyWage < taxFreePersonalAllowanceYearly)
        {
            netIncomeYearly = grossYearlyWage;
            monthlyIncome = grossYearlyWage / 12;
        }
        else
        {
            var taxableIncome = grossYearlyWage - taxFreePersonalAllowanceYearly;

            var taxableIncomeTmp = Money.From(taxableIncome.Amount);
            var totalTaxPayable = Money.From(0);
            foreach (var taxRatesAndbands in EnglandNorthernIrelandAndWalesTaxBands.TaxRatesAndBands.Skip(1))
            {
                var amountToRate = taxRatesAndbands.MaxTaxableIncome - taxRatesAndbands.MinTaxableIncome + Money.From(1);
                if (taxableIncomeTmp < amountToRate)
                {
                    totalTaxPayable += taxableIncomeTmp * taxRatesAndbands.Rate / 100;
                    break;
                }

                totalTaxPayable += amountToRate * taxRatesAndbands.Rate / 100;
                taxableIncomeTmp -= amountToRate;
            }

            netIncomeYearly = grossYearlyWage - totalTaxPayable;
            monthlyIncome = netIncomeYearly / 12;
        }
        var wagesPostTax = Enumerable.Repeat(monthlyIncome, 11).ToList();
        wagesPostTax.Add(netIncomeYearly - monthlyIncome * 11);

        return wagesPostTax;
    }
}

public enum IncomeFrequency
{
    Unknown = 0,
    Yearly = 1,
    Monthly = 2,
    Every4Weeks = 3,
    Weekly = 4,
    Daily = 5,
    Hourly = 6,
}

public class TaxRatesAndBands(string bandName, Money minTaxableIncome, Money maxTaxableIncome, decimal rate)
{
    public string BandName { get; } = bandName;
    public Money MinTaxableIncome { get; } = minTaxableIncome;
    public Money MaxTaxableIncome { get; } = maxTaxableIncome;
    public decimal Rate { get; } = rate;
}

public static class EnglandNorthernIrelandAndWalesTaxBands
{
    public static List<TaxRatesAndBands> TaxRatesAndBands =
    [
        new TaxRatesAndBands("Personal Allowance", Money.From(1), Money.From(12570), 0), // ewww 1 as min??
        new TaxRatesAndBands("Basic Rate", Money.From(12571), Money.From(50270), 20),
        new TaxRatesAndBands("Higher Rate", Money.From(50271), Money.From(125140), 40),
        new TaxRatesAndBands("Additional Rate", Money.From(125141), Money.From(9999999999), 45),
    ];
}
