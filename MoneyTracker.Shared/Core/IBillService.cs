using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.Shared.Core;
public interface IBillService
{
    Task<List<BillResponseDTO>> AddBill(NewBillRequestDTO newBill);
    Task<List<BillResponseDTO>> DeleteBill(DeleteBillRequestDTO deleteBill);
    Task<List<BillResponseDTO>> EditBill(EditBillRequestDTO editBill);
    Task<List<BillResponseDTO>> GetAllBills();
}
