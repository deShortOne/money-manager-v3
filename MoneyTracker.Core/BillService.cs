using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Core;
public class BillService : IBillService
{
    private readonly IBillDatabase _dbService;

    public BillService(IBillDatabase dbService)
    {
        _dbService = dbService;
    }

    public Task<List<BillDTO>> GetAllBills()
    {
        return _dbService.GetAllBills();
    }

    public Task<List<BillDTO>> AddBill(NewBillDTO newBill)
    {
        return _dbService.AddBill(newBill);
    }

    public Task<List<BillDTO>> EditBill(EditBillDTO editBill)
    {
        return _dbService.EditBill(editBill);
    }

    public Task<List<BillDTO>> DeleteBill(DeleteBillDTO deleteBill)
    {
        return _dbService.DeleteBill(deleteBill);
    }
}
