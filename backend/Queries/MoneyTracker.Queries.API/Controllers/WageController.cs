using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;
using MoneyTracker.Queries.Domain.Handlers;

namespace MoneyTracker.Queries.API.Controllers;
[ApiController]
[Route("[controller]")]
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
        Pension? pension = null;
        if (wageRequest.Pension is not null)
        {
            var pensionResult = TryGeneratePensionRequest(wageRequest.Pension);
            if (!pensionResult.IsSuccess)
            {
                return new ContentResult
                {
                    Content = pensionResult.Error?.Description,
                    ContentType = "text/plain",
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
            pension = pensionResult.Value;
        }

        var request = new CalculateWageRequest(
            Money.From(wageRequest.GrossIncome),
            wageRequest.FrequencyOfIncome,
            wageRequest.TaxCode,
            wageRequest.PayNationalInsurance,
            pension,
            wageRequest.StudentLoanOptions
        );

        return ControllerHelper.Convert(_wageService.CalculateWage(request));
    }

    private static ResultT<Pension> TryGeneratePensionRequest(Public.Pension pension)
    {
        var pensionCalculatorResult = TryGeneratePensionCalculator(pension);
        if (!pensionCalculatorResult.IsSuccess)
        {
            return pensionCalculatorResult.Error!;
        }

        if (!Enum.TryParse<PensionType>(pension.Type, out var pensionType))
        {
            var pensionTypes = new List<PensionType>(Enum.GetValues<PensionType>())
                .Where(x => x != PensionType.Unknown);
            return Error.Validation("", $"Invalid pension type \"{pension.Type}\", valid pension types are: {string.Join(", ", pensionTypes)}");
        }

        return new Pension(pensionCalculatorResult.Value, pensionType);
    }

    private static ResultT<IPensionCalculator> TryGeneratePensionCalculator(Public.Pension pension)
    {
        if (!Enum.TryParse<Public.PensionCalculationType>(pension.Rate, out var requestedPensionCalculatedType))
        {
            var pensionTypes = new List<Public.PensionCalculationType>(Enum.GetValues<Public.PensionCalculationType>())
                .Where(x => x != Public.PensionCalculationType.Unknown);
            return Error.Validation("", $"Invalid pension rate \"{pension.Rate}\", valid rates are: {string.Join(", ", pensionTypes)}");
        }
        if (requestedPensionCalculatedType == Public.PensionCalculationType.Percentage)
        {
            return new PercentagePensionAmount(Percentage.From(pension.Value));
        }
        if (requestedPensionCalculatedType == Public.PensionCalculationType.Amount)
        {
            return new FixedPensionAmount(Money.From(pension.Value));
        }

        return Error.NotFound("", "Pension calculation type found but no implementation has been found.");
    }
}
