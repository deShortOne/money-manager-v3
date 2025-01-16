
namespace MoneyTracker.Contracts.Requests.Bill;
public class DeleteBillRequest(int id)
{
    public int Id { get; } = id;
}
