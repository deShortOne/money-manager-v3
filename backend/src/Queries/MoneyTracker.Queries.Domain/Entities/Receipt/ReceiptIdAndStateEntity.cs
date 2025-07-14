
using MoneyTracker.Common.Values;

namespace MoneyTracker.Queries.Domain.Entities.Receipt;
public class ReceiptIdAndStateEntity(string id, ReceiptState state)
{
    public string Id { get; } = id;
    public ReceiptState State { get; } = state;
}
