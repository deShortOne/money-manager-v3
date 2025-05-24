using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class WageController
{
    private readonly IWageService _wageService;

    public WageController(IWageService wageService)
    {
        _wageService = wageService;
    }

    [HttpPost]
    [Route("calculate")]
    public async Task<IActionResult> CalculateWage(Public.CalculateWageRequest wageRequest)
    {
        await Task.CompletedTask;
        if (!TryGeneratePensionRequest(wageRequest, out var pension))
        {
            return new ContentResult
            {
                Content = $"Invalid Pension object: {wageRequest.Pension}",
                ContentType = "text/plain",
                StatusCode = StatusCodes.Status400BadRequest,
            };
        }

        var request = new CalculateWageRequest(
            wageRequest.GrossIncome,
            wageRequest.FrequencyOfIncome,
            wageRequest.TaxCode,
            wageRequest.PayNationalInsurance,
            pension,
            wageRequest.StudentLoanOptions
        );

        return ControllerHelper.Convert(_wageService.CalculateWage(request));
    }

    private static bool TryGeneratePensionRequest(Public.CalculateWageRequest wageRequest, out Pension? pensionObject)
    {
        if (!TryGeneratePensionCalculator(wageRequest, out var pensionCalculator))
        {
            pensionObject = null;
            return false;
        }

        if (!Enum.TryParse<PensionType>(wageRequest.Pension.PensionType, out var pensionType))
        {
            pensionObject = null;
            return false;
        }

        pensionObject = new Pension(pensionCalculator, pensionType);
        return true;
    }

    private static bool TryGeneratePensionCalculator(Public.CalculateWageRequest wageRequest, out IPensionCalculator? pensionCalculator)
    {
        if (!Enum.TryParse<Public.PensionCalculationType>(wageRequest.Pension.PensionCalculationType, out var requestedPensionCalculatedType))
        {
            pensionCalculator = null;
            return false;
        }

        if (requestedPensionCalculatedType == Public.PensionCalculationType.Percentage)
        {
            pensionCalculator = new PercentagePensionAmount(Percentage.From(wageRequest.Pension.Value));
        }
        else if (requestedPensionCalculatedType == Public.PensionCalculationType.Amount)
        {
            pensionCalculator = new FixedPensionAmount(Money.From(wageRequest.Pension.Value));
        }
        else
        {
            pensionCalculator = null;
            return false;
        }
        return true;
    }
}
