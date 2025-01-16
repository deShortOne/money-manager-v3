using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Bill;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IBillCommandRepository
{
    public Task AddBill(BillEntity bill);
    public Task EditBill(EditBillEntity editBillDTO);
    public Task DeleteBill(int billIdToDelete);
    public Task<BillEntity?> GetBillById(int id);
    public Task<int> GetLastId();
}
