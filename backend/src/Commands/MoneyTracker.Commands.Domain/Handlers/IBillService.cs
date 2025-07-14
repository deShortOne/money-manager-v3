using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Bill;

namespace MoneyTracker.Commands.Domain.Handlers;
public interface IBillService
{
    Task<Result> AddBill(string token, NewBillRequest newBill, CancellationToken cancellationToken);
    Task<Result> DeleteBill(string token, DeleteBillRequest deleteBill, CancellationToken cancellationToken);
    Task<Result> EditBill(string token, EditBillRequest editBill, CancellationToken cancellationToken);
    Task<Result> SkipOccurence(string token, SkipBillOccurrenceRequest skipBillDTO, CancellationToken cancellationToken);
}
