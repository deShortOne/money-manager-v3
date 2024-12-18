
namespace MoneyTracker.Contracts.Requests.Transaction;
public class DeleteTransactionRequest(int id)
{
    public int Id { get; } = id;
}
