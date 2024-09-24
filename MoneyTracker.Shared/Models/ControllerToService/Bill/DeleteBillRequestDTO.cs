
namespace MoneyTracker.Shared.Models.ControllerToService.Bill;
public class DeleteBillRequestDTO(int id)
{
    public int Id { get; private set; } = id;
}
