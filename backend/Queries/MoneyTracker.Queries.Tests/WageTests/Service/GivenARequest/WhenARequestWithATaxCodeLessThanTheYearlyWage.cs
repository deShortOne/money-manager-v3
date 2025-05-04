using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenARequestWithATaxCodeLessThanTheYearlyWage
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenARequestWithATaxCodeLessThanTheYearlyWage()
    {
        var request = new CalculateWageRequest(12000, "Yearly", "1257L", false);
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
        Assert.Equal(Enumerable.Repeat(Money.From(1000), 12), wages);
    }
}
