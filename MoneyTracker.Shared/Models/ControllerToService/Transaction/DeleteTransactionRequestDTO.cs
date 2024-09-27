namespace MoneyTracker.Shared.Models.ControllerToService.Transaction;

public class DeleteTransactionRequestDTO(int id)
{
    public int Id { get; } = id;
}
