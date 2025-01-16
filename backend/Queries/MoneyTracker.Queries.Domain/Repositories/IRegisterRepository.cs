using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Queries.Domain.Entities.Transaction;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IRegisterRepository
{
    public Task<List<TransactionEntity>> GetAllTransactions(AuthenticatedUser user);
}
