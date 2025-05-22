using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.Application;
public class WageService : IWageService
{
    public ResultT<CalculateWageResponse> CalculateWage(CalculateWageRequest request)
    {
        var incomeFrequency = Convert(request.FrequencyOfIncome);
        if (incomeFrequency is Frequency.Unknown)
        {
            return Error.Validation("", "Invalid frequency");
        }

        var grossYearlyWage = CalculateGrossYearlyWage(request.GrossIncome, incomeFrequency);
        var response = new CalculateWageResponse
        {
            GrossYearlyIncome = grossYearlyWage,
            Wages = GetWageReducedbyTax(grossYearlyWage, request),
        };

        return response;
    }

    private static Frequency Convert(string incomeFrequency)
    {
        Enum.TryParse(incomeFrequency, out Frequency frequencyEnum);
        return frequencyEnum;
    }

    private static Money CalculateGrossYearlyWage(decimal grossIncome, Frequency frequencyOfIncome)
    {
        return frequencyOfIncome switch
        {
            Frequency.Yearly => Money.From(grossIncome),
            Frequency.Monthly => Money.From(grossIncome * 12),
            Frequency.Every4Weeks => Money.From(grossIncome * 13),
            Frequency.Weekly => Money.From(grossIncome * 52),
            Frequency.Daily => throw new NotImplementedException(),
            // Implement number of days of week, grossIncome * numWorkDaysPerWeek * 52
            Frequency.Hourly => throw new NotImplementedException(),
            // Implement number of hours of week, grossIncome * numWorkHoursPerWeek * 52
            _ => throw new NotImplementedException(),
        };
    }

    private static List<Money> GetWageReducedbyTax(Money grossYearlyWage, CalculateWageRequest request)
    {
        var builder = SalaryCalculatorBuilder.CreateBuilder(request);
        var result = builder.CalculateYearlyWage(grossYearlyWage);

        var yearlyWage = grossYearlyWage - result.TotalDeduction;
        var netIncomeMonthly = yearlyWage / 12;
        var wagesPostTax = Enumerable.Repeat(netIncomeMonthly, 11).ToList();
        wagesPostTax.Add(yearlyWage - netIncomeMonthly * 11);

        return wagesPostTax;
    }
}
