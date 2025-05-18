using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.Pension;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenAValidRequestWithAllOptionsRequested
{
    [Fact]
    public void ThenAValidResponseIsReturned()
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(
            30000,
            "Yearly",
            "1257L",
            true,
            new FixedPensionAmount(Money.From(100)),
            new StudentLoanOptions(false, true, false, false, true));
        var expected = new CalculateWageResponse
        {
            GrossYearlyIncome = Money.From(30000),
            Wages = Enumerable.Repeat(Money.From(1937.3m), 12).ToList(),
        };

        var actual = calculateWageService.CalculateWage(request);

        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, actual.Value);
    }
}
