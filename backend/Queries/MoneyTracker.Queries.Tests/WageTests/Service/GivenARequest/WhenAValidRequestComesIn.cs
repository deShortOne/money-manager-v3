using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAValidRequest;
public sealed class WhenAValidRequestComesIn
{
    public static TheoryData<decimal, string, decimal, List<decimal>> ValidRequests = new()
    {
        { 18000, "Yearly", 18000, new List<decimal> { 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500, } },
        { 2000, "Monthly", 24000, new List<decimal> { 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, 2000, } },
        { 3000, "Every4Weeks", 39000, new List<decimal> { 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, 3250, } },
        { 600, "Weekly", 31200, new List<decimal> { 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, 2600, } },
    };

    [Theory, MemberData(nameof(ValidRequests))]
    public void ThenAValidResponseIsReturned(decimal grossIncome, string incomeFrequency, decimal expectedGrossYearlyIncome, List<decimal> expectedMonthlyIncome)
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(grossIncome, incomeFrequency);
        var expected = new CalculateWageResponse();
        expected.GrossYearlyIncome = expectedGrossYearlyIncome;
        expected.Wages = expectedMonthlyIncome;

        var actual = calculateWageService.CalculateWage(request);

        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, actual.Value);
    }
}
