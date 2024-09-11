using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.DateManager;
using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Core;
public class BillService : IBillService
{
    private readonly IBillDatabase _dbService;
    private readonly IDateProvider _dateProvider;

    public BillService(IBillDatabase dbService, IDateProvider dateProvider)
    {
        _dbService = dbService;
        _dateProvider = dateProvider;
    }

    public async Task<List<BillDTO>> GetAllBills()
    {
        var databaseBills = await _dbService.GetAllBills();

        List<BillDTO> res = [];
        foreach (var bill in databaseBills)
        {
            res.Add(bill);
        }

        return res;
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
