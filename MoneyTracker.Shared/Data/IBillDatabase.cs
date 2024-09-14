
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Shared.Data;
public interface IBillDatabase
{
    public Task<List<BillFromRepositoryDTO>> GetAllBills();
    public Task<List<BillFromRepositoryDTO>> AddBill(NewBillDTO bill);
    public Task<List<BillFromRepositoryDTO>> EditBill(EditBillDTO editBillDTO);
    public Task<List<BillFromRepositoryDTO>> DeleteBill(DeleteBillDTO editBillDTO);
    public Task<BillFromRepositoryDTO> GetBillById(int id);
}
