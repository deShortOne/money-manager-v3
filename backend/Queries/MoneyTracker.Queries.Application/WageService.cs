using System.Text.RegularExpressions;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.Application;
public class WageService : IWageService
{
    public static readonly Percentage UkNationalInsuranceTax = Percentage.From(8);

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
            Wages = GetWageReducedByTax(grossYearlyWage, request),
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

    private static List<Money> GetWageReducedByTax(Money grossYearlyWage, CalculateWageRequest request)
    {
        var taxCodeElements = Regex.Match(request.TaxCode, @"^(\d+)(\w+)$");
        var personalAllowanceAmount = Money.From(int.Parse(taxCodeElements.Groups[1].Value) * 10);
        var taxLetter = taxCodeElements.Groups[2].Value.ToUpper();

        if (taxLetter != "L")
        {
            throw new NotImplementedException("Only tax letter L is accepted");
        }

        var totalTaxPayable = Money.Zero;
        var taxableIncome = Money.From(grossYearlyWage) - personalAllowanceAmount;
        var taxableIncomeRemaining = Money.From(taxableIncome);
        if (taxableIncomeRemaining > Money.Zero)
        {
            foreach (var taxRatesAndbands in EnglandNorthernIrelandAndWalesTaxBands.TaxRatesAndBands.Skip(1))
            {
                var amountToRate = taxRatesAndbands.MaxTaxableIncome - taxRatesAndbands.MinTaxableIncome + Money.From(1);
                if (taxableIncomeRemaining <= amountToRate)
                {
                    totalTaxPayable += taxableIncomeRemaining * taxRatesAndbands.Rate;
                    break;
                }

                totalTaxPayable += amountToRate * taxRatesAndbands.Rate;
                taxableIncomeRemaining -= amountToRate;
            }
        }

        var netIncomeYearly = grossYearlyWage - totalTaxPayable;
        if (request.PayNationalInsurance)
            netIncomeYearly -= taxableIncome * UkNationalInsuranceTax;
        var netIncomeMonthly = netIncomeYearly / 12;

        var wagesPostTax = Enumerable.Repeat(netIncomeMonthly, 11).ToList();
        wagesPostTax.Add(netIncomeYearly - netIncomeMonthly * 11);

        return wagesPostTax;
    }

    public static Money CalculateStudentLoan(Money grossYearlyWage, StudentLoanOptions studentLoanOptions)
    {
        grossYearlyWage /= 12;
        var result = Money.Zero;
        if (studentLoanOptions.Plan5)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[3].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[3].Rate;
                result = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (studentLoanOptions.Plan1)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[0].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[0].Rate;
                result = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (studentLoanOptions.Plan2)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[1].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[1].Rate;
                result = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (studentLoanOptions.Plan4)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[2].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[2].Rate;
                result = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        if (studentLoanOptions.PostGraduate)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[4].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[4].Rate;
                result += Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }

        return result;
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

public enum StudentLoanPlan
{
    Unknown = 0,
    Plan1 = 1,
    Plan2 = 2,
    Plan4 = 3,
    Plan5 = 4,
    PostgraduateLoan = 5,
}

public class StudentLoanBand(StudentLoanPlan plan, Money yearlyIncomeThreshold, Money monthlyIncomeThreshold,
    Money weeklyIncomeThreshold, Percentage rate)
{
    public StudentLoanPlan StudentLoanPlan { get; } = plan;
    public Money YearlyIncomeThreshold { get; } = yearlyIncomeThreshold;
    public Money MonthlyIncomeThreshold { get; } = monthlyIncomeThreshold;
    public Money WeeklyIncomeThreshold { get; } = weeklyIncomeThreshold;
    public Percentage Rate { get; } = rate;
}

public static class UkStudentLoanRepayment
{
    public static List<StudentLoanBand> StudentLoanBands =
    [
        new StudentLoanBand(StudentLoanPlan.Plan1, Money.From(26065), Money.From(2172), Money.From(501), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan2, Money.From(28470), Money.From(2372), Money.From(547), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan4, Money.From(32745), Money.From(2728), Money.From(629), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan5, Money.From(25000), Money.From(2083), Money.From(480), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.PostgraduateLoan, Money.From(21000), Money.From(1750), Money.From(403), Percentage.From(6)),
    ];
}
