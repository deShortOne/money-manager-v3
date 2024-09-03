
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Data.Global;
public interface IBillDatabase
{
    public Task<List<BillDTO>> GetBill();
    public Task<List<BillDTO>> AddBill(NewBillDTO bill);
    public Task<List<BillDTO>> EditBill(EditBillDTO editBillDTO);
}
