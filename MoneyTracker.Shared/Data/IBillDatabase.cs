
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;

namespace MoneyTracker.Shared.Data;
public interface IBillDatabase
{
    public Task<List<BillEntityDTO>> GetAllBills();
    public Task<List<BillEntityDTO>> AddBill(NewBillDTO bill);
    public Task<List<BillEntityDTO>> EditBill(EditBillDTO editBillDTO);
    public Task<List<BillEntityDTO>> DeleteBill(DeleteBillDTO editBillDTO);
    public Task<BillEntityDTO> GetBillById(int id);
}
