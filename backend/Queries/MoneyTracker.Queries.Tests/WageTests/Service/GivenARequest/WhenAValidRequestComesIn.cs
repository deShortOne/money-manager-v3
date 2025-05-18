using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAValidRequest;
public sealed class WhenAValidRequestComesIn
{
    public static TheoryData<decimal, string, string, decimal, List<decimal>> ValidRequests = new()
    {
        { 18000, "Yearly", "9999L", 18000, new List<decimal> { 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, } },
        { 2000, "Monthly", "9999L", 24000, new List<decimal> { 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, } },
        { 3000, "Every4Weeks", "9999L", 39000, new List<decimal> { 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, } },
        { 600, "Weekly", "9999L", 31200, new List<decimal> { 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, } },
        { 19000, "Yearly", "9999L", 19000, new List<decimal> { 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.33m, 1583.37m, } },
    };

    [Theory, MemberData(nameof(ValidRequests))]
    public void ThenAValidResponseIsReturned(decimal grossIncome, string incomeFrequency, string taxCode, decimal expectedGrossYearlyIncome, List<decimal> expectedMonthlyIncome)
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(
            grossIncome,
            incomeFrequency,
            taxCode,
            false,
            null,
            new StudentLoanOptions(false, false, false, false, false));
        var expected = new CalculateWageResponse
        {
            GrossYearlyIncome = Money.From(expectedGrossYearlyIncome),
            Wages = expectedMonthlyIncome.Select(Money.From).ToList()
        };

        var actual = calculateWageService.CalculateWage(request);

        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, actual.Value);
    }
}
