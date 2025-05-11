using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenTheYearlyWageIsOnTheHigherRate
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenTheYearlyWageIsOnTheHigherRate()
    {
        var request = new CalculateWageRequest(50270,
            "Yearly",
            "1257L",
            false,
            new StudentLoanOptions(false, false, false, false, false));
        var service = new WageService();
        _subject = service.CalculateWage(request);
    }

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_subject.IsSuccess);
    }

    [Fact]
    public void ThenTheWagesAreCorrect()
    { // only taxed at 20%
        var wages = _subject.Value.Wages;
        var expectedWages = new List<Money>
        {
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.83m),
            Money.From(3560.87m),
        };

        Assert.Equal(expectedWages, wages);
    }
}
