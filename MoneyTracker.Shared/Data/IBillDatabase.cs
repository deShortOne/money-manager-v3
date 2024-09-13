
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Shared.Data;
public interface IBillDatabase
{
    public Task<List<BillDTO>> GetAllBills();
    public Task<List<BillDTO>> AddBill(NewBillDTO bill);
    public Task<List<BillDTO>> EditBill(EditBillDTO editBillDTO);
    public Task<List<BillDTO>> DeleteBill(DeleteBillDTO editBillDTO);
    Task<BillDTO> GetBillById(int id);
}
