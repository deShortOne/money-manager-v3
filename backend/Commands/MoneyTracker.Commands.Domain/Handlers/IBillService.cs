using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Bill;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBillService
{
    Task<Result> AddBill(string token, NewBillRequest newBill);
    Task DeleteBill(string token, DeleteBillRequest deleteBill);
    Task<Result> EditBill(string token, EditBillRequest editBill);
    Task SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO);
}
