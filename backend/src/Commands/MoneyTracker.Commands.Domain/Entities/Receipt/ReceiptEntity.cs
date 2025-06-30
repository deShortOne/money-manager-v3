
namespace MoneyTracker.Commands.Domain.Entities.Receipt;
public class ReceiptEntity(string id, int userId, string name, string url, int state)
{
    public string Id { get; } = id;
    public int UserId { get; } = userId;
    public string Name { get; } = name;
    public string Url { get; } = url;
    public int State { get; private set; } = state;

    public void UpdateState(int state)
    {
        State = state;
    }
}
