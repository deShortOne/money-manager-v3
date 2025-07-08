
namespace MoneyTracker.Queries.Domain.Entities.Receipt;
public class ReceiptIdAndStateEntity(string id, int state)
{
    public string Id { get; } = id;
    public int State { get; } = state;
}
