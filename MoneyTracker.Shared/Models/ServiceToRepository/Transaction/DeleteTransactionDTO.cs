namespace MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

public class DeleteTransactionDTO(int id)
{
    public int Id { get; } = id;
}
