using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenOnlyThePensionPecentageIsRequested
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenOnlyThePensionPecentageIsRequested()
    {
        var calculateWageService = new WageService();
        var request = new CalculateWageRequest(1000,
            "Monthly",
            "9999L",
            false,
            new Pension(10, PensionType.Percentage),
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
        Assert.Equal(Enumerable.Repeat(Money.From(900), 12).ToList(), _subject.Value.Wages);
    }
}
