
namespace MoneyTracker.Commands.Domain.Entities.Receipt;
public class ReceiptEntity(string id, int userId, string name, string url, int state, int? finalTransactionId = null)
{
    public string Id { get; } = id;
    public int UserId { get; } = userId;
    public string Name { get; } = name;
    public string Url { get; } = url;
    public int State { get; private set; } = state;
    public int? FinalTransactionId { get; private set; } = finalTransactionId;

    public void UpdateState(int state)
    {
        State = state;
    }

    public void SetFinalTransactionId(int finalTransactionId)
    {
        FinalTransactionId = finalTransactionId;
    }
}
