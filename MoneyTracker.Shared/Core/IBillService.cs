using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Shared.Core;
public interface IBillService
{
    Task<List<BillDTO>> AddBill(NewBillDTO newBill);
    Task<List<BillDTO>> DeleteBill(DeleteBillDTO deleteBill);
    Task<List<BillDTO>> EditBill(EditBillDTO editBill);
    Task<List<BillDTO>> GetAllBills();
}
