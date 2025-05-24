using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Contracts.Responses.Wage;
using MoneyTracker.Queries.API.Controllers;
using MoneyTracker.Queries.Domain.Handlers;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Controller.GivenARequestToTheWageController;
public sealed class WhenTheRequestIsValid : IAsyncLifetime
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
    public void ThenThereIsOnlyOneRequestPassedToTheService()
    {
        Assert.Single(_requestsPassedIntoService);
    }

    [Fact]
    public void ThenTheGrossIncomeIsMappedCorrectly()
    {
        Assert.Equal(1000, _requestsPassedIntoService[0].GrossIncome);
    }

    [Fact]
    public void ThenTheFrequencyOfIncomeIsMappedCorrectly()
    {
        Assert.Equal("Yearly", _requestsPassedIntoService[0].FrequencyOfIncome);
    }

    [Fact]
    public void ThenTheTaxCodeIsMappedCorrectly()
    {
        Assert.Equal("1575M", _requestsPassedIntoService[0].TaxCode);
    }

    [Fact]
    public void ThenTheRequestToIncludeNationalInsuranceIsMappedCorrectly()
    {
        Assert.True(_requestsPassedIntoService[0].PayNationalInsurance);
    }

    [Fact]
    public void ThenThePensionTypeIsMappedCorrectly()
    {
        Assert.NotNull(_requestsPassedIntoService[0].Pension);
        Assert.Equal(PensionType.AutoEnrolment, _requestsPassedIntoService[0].Pension.Type);
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

    [Fact]
    public void ThenTheStudentLoanIsMappedCorrectly()
    {
        Assert.NotNull(_requestsPassedIntoService[0].StudentLoanOptions);
        Assert.Multiple(() =>
        {
            var studentLoanOptions = _requestsPassedIntoService[0].StudentLoanOptions;
            Assert.False(studentLoanOptions.Plan1);
            Assert.True(studentLoanOptions.Plan2);
            Assert.False(studentLoanOptions.Plan4);
            Assert.False(studentLoanOptions.Plan5);
            Assert.True(studentLoanOptions.PostGraduate);

        });
    }
}
