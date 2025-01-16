using MoneyTracker.Contracts.Responses.Bill;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IBillService
{
    Task<List<BillResponse>> GetAllBills(string token);
    Task<List<string>> GetAllFrequencyNames();
}
