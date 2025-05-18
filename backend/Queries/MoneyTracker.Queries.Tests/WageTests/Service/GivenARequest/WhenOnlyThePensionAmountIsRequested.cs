using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.Pension;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenOnlyThePensionAmountIsRequested
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenOnlyThePensionAmountIsRequested()
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(1000,
            "Monthly",
            "9999L",
            false,
            new FixedPensionAmount(Money.From(200)),
            new StudentLoanOptions(false, false, false, false, false));

        _subject = calculateWageService.CalculateWage(request);
    }

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_subject.IsSuccess);
    }

    [Fact]
    public void ThenThePensionAmountIsTakenOff()
    {
        Assert.Equal(Enumerable.Repeat(Money.From(800), 12).ToList(), _subject.Value.Wages);
    }
}
