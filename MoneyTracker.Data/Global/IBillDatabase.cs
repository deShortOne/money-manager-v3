
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Data.Global;
public interface IBillDatabase
{
    public Task<List<BillDTO>> GetBill();
}
