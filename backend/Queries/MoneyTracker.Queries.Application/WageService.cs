using MoneyTracker.Common.Result;
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

        var response = new CalculateWageResponse();
        response.GrossYearlyIncome = grossYearlyWage;
        var monthlyIncome = (decimal)(int)(grossYearlyWage / 12 * 100) / 100;
        response.Wages = Enumerable.Repeat(monthlyIncome, 11).ToList();
        response.Wages.Add(grossYearlyWage - monthlyIncome * 11);

        return response;
    }

    private static IncomeFrequency Convert(string incomeFrequency)
    {
        Enum.TryParse(incomeFrequency, out IncomeFrequency incomeFrequencyEnum);
        return incomeFrequencyEnum;
    }

    private static decimal CalculateGrossYearlyWage(decimal grossIncome, IncomeFrequency frequencyOfIncome)
    {
        return frequencyOfIncome switch
        {
            IncomeFrequency.Yearly => grossIncome,
            IncomeFrequency.Monthly => grossIncome * 12,
            IncomeFrequency.Every4Weeks => grossIncome * 13,
            IncomeFrequency.Weekly => grossIncome * 52,
            IncomeFrequency.Daily => throw new NotImplementedException(),
            // Implement number of days of week, grossIncome * numWorkDaysPerWeek * 52
            IncomeFrequency.Hourly => throw new NotImplementedException(),
            // Implement number of hours of week, grossIncome * numWorkHoursPerWeek * 52
            _ => throw new NotImplementedException(),
        };
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
