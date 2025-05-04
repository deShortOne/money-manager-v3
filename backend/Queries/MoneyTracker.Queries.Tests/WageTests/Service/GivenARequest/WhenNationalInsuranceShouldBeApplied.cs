using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenARequest;
public sealed class WhenNationalInsuranceShouldBeApplied
{
    private ResultT<CalculateWageResponse> _subject;

    public WhenNationalInsuranceShouldBeApplied()
    {
        var request = new CalculateWageRequest(19000, "Yearly", "1257L", true);
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
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
            Money.From(1433.3m),
        };

        Assert.Equal(expectedWages, wages);
    }
}
