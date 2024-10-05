using MoneyTracker.Shared.Models.ControllerToService.Bill;
using MoneyTracker.Shared.Models.ServiceToController.Bill;

namespace MoneyTracker.Shared.Core;
public interface IBillService
{
    Task AddBill(string token, NewBillRequestDTO newBill);
    Task DeleteBill(string token, DeleteBillRequestDTO deleteBill);
    Task EditBill(string token, EditBillRequestDTO editBill);
    Task<List<BillResponseDTO>> GetAllBills(string token);
    Task SkipOccurence(string token, SkipBillOccurrenceRequestDTO skipBillDTO);
}
