using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Contracts.Responses.Wage;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IWageService
{
    ResultT<CalculateWageResponse> CalculateWage(CalculateWageRequest request);
}
