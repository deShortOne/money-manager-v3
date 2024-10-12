
using MoneyTracker.Shared.Auth;
using MoneyTracker.Shared.Models.RepositoryToService.Bill;
using MoneyTracker.Shared.Models.ServiceToRepository.Bill;

namespace MoneyTracker.Shared.Data;
public interface IBillDatabase
{
    public Task<List<BillEntityDTO>> GetAllBills(AuthenticatedUser user);
    public Task AddBill(BillEntity bill);
    public Task EditBill(EditBillEntity editBillDTO);
    public Task DeleteBill(int billIdToDelete);
    public Task<BillEntityDTO> GetBillById(AuthenticatedUser user, int id);
    Task<bool> IsBillAssociatedWithUser(AuthenticatedUser user, int billId);
    public Task<int> GetLastId();
}
