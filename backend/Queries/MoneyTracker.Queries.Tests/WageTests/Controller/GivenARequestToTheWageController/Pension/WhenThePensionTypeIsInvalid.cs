using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.API.Controllers;
using MoneyTracker.Queries.Domain.Handlers;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Controller.GivenARequestToTheWageController.Pension;
public sealed class WhenThePensionTypeIsInvalid : IAsyncLifetime
{
    private ContentResult _calculateWageResponse;

    public async Task InitializeAsync()
    {
        var request = new API.Public.CalculateWageRequest
        (
            1000,
            "Yearly",
            "1575M",
            true,
            new API.Public.Pension("invalid type", 100, "Amount"),
            new StudentLoanOptions(false, true, false, false, true)
        );

        var wageCalculatorMock = new Mock<IWageService>();

        var wageController = new WageController(wageCalculatorMock.Object);

        _calculateWageResponse = (ContentResult)await wageController.CalculateWage(request);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void ThenAnErrorIsReturned()
    {
        Assert.Equal(400, _calculateWageResponse.StatusCode);
    }

    [Fact]
    public void ThenTheErrorMessageIsCorrect()
    {
        Assert.Equal("Invalid pension type \"invalid type\", valid pension types are: AutoEnrolment, Personal", _calculateWageResponse.Content);
    }
}
