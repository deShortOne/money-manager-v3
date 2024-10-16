using MoneyTracker.Contracts.Requests.Bill;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBillService
{
    Task AddBill(string token, NewBillRequest newBill);
    Task DeleteBill(string token, DeleteBillRequest deleteBill);
    Task EditBill(string token, EditBillRequest editBill);
    Task SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO);
}
