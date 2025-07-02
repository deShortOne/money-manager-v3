using MoneyTracker.Commands.Domain.Entities.Bill;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IBillCommandRepository
{
    public Task AddBill(BillEntity bill, CancellationToken cancellationToken);
    public Task EditBill(EditBillEntity editBillDTO, CancellationToken cancellationToken);
    public Task DeleteBill(int billIdToDelete, CancellationToken cancellationToken);
    public Task<BillEntity?> GetBillById(int id, CancellationToken cancellationToken);
    public Task<int> GetLastId(CancellationToken cancellationToken);
}
