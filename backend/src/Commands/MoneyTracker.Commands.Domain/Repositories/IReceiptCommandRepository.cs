
using MoneyTracker.Commands.Domain.Entities.Receipt;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IReceiptCommandRepository
{
    Task AddReceipt(ReceiptEntity receipt);
}
