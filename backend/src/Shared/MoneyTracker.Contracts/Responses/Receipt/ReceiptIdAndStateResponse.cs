using MoneyTracker.Common.Values;

namespace MoneyTracker.Contracts.Responses.Receipt;
public class ReceiptIdAndStateResponse(string id, ReceiptState state)
{
    public string Id { get; } = id;
    public ReceiptState State { get; } = state;
}
