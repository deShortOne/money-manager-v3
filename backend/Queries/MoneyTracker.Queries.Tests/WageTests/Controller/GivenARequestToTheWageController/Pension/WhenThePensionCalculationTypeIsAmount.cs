using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.API.Controllers;
using MoneyTracker.Queries.Domain.Handlers;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Controller.GivenARequestToTheWageController.Pension;
public sealed class WhenThePensionCalculationTypeIsAmount : IAsyncLifetime
{
    private readonly List<CalculateWageRequest> _requestsPassedIntoService = new List<CalculateWageRequest>();

    public async Task InitializeAsync()
    {
        var request = new API.Public.CalculateWageRequest
        (
            1000,
            "Yearly",
            "1575M",
            true,
            new API.Public.Pension("AutoEnrolment", 1234, "Amount"),
            new StudentLoanOptions(false, true, false, false, true)
        );

        var wageCalculatorMock = new Mock<IWageService>();
        wageCalculatorMock
            .Setup(x => x.CalculateWage(It.IsAny<CalculateWageRequest>()))
            .Callback((CalculateWageRequest requestToService) => _requestsPassedIntoService.Add(requestToService))
            .Returns(new CalculateWageResponse());

        var wageController = new WageController(wageCalculatorMock.Object);

        await wageController.CalculateWage(request);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void ThenThePensionCalculatorIsMappedCorrectly()
    {
        var pension = _requestsPassedIntoService[0].Pension;
        Assert.NotNull(pension);

        Assert.IsType<FixedPensionAmount>(pension.Calculator);

        var pensionValue = ((FixedPensionAmount)pension.Calculator).Amount;
        Assert.Equal(Money.From(1234), pensionValue);
    }
}
