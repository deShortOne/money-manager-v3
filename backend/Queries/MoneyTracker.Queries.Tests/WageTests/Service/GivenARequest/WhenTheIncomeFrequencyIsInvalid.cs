using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenTheIncomeFrequencyIsInvalid
{
    public static TheoryData<string> InvalidRequests = new()
    {
        { "YearLY" },
        { "Monthy" },
        { "Every_4_Weeks" },
        { "Weeklyasdf" },
    };

    [Theory, MemberData(nameof(InvalidRequests))]
    public void ThenAValidResponseIsReturned(string incomeFrequency)
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(12345, incomeFrequency, "9999L");

        var actual = calculateWageService.CalculateWage(request);

        Assert.False(actual.IsSuccess);
        Assert.Equal("Invalid frequency", actual.Error?.Description);
    }
}
