using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Bill;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IBillService
{
    Task<ResultT<List<BillResponse>>> GetAllBills(string token);
    Task<List<string>> GetAllFrequencyNames();
}
