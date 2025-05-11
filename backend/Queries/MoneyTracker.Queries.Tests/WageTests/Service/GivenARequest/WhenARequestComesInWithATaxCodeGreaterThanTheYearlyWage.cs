using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenARequestComesInWithATaxCodeGreaterThanTheYearlyWage
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenARequestComesInWithATaxCodeGreaterThanTheYearlyWage()
    {
        var request = new CalculateWageRequest(
            19000,
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
    {
        var wages = _subject.Value.Wages;
        var expectedWages = new List<Money>
        {
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.16m),
            Money.From(1476.24m),
        };

        Assert.Equal(expectedWages, wages);
    }
}
