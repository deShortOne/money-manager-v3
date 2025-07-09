using System.Text.Json.Serialization;
using MoneyTracker.Common.Values;

namespace MoneyTracker.Contracts.Responses.Receipt;
public class ReceiptIdAndStateResponse(string id, ReceiptState state)
{
    [JsonPropertyName("id")]
    public string Id { get; } = id;
    [JsonPropertyName("state")]
    public ReceiptState State { get; } = state;
}
