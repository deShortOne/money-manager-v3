using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Services.Infrastructure;
public class BillDbService
{
    private readonly IBillDatabase _database;

    public BillDbService(IBillDatabase db)
    {
        _database = db;
    }

    public Task<List<BillDTO>> GetAllBills()
    {
        return _database.GetBill();
    }

    public Task<List<BillDTO>> AddBill(NewBillDTO newBill)
    {
        return _database.AddBill(newBill);
    }

    public Task<List<BillDTO>> EditBill(EditBillDTO editBill)
    {
        return _database.EditBill(editBill);
    }

    public Task<List<BillDTO>> DeleteBill(DeleteBillDTO deleteBill)
    {
        return _database.DeleteBill(deleteBill);
    }
}
