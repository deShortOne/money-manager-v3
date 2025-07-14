
using MoneyTracker.Common.Values;

namespace MoneyTracker.Queries.Domain.Entities.Receipt;
public class ReceiptEntity(string id, int userId, string name, string url, ReceiptState state)
{
    public string Id { get; } = id;
    public int UserId { get; } = userId;
    public string Name { get; } = name;
    public string Url { get; } = url;
    public ReceiptState State { get; } = state;
}
