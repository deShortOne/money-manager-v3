using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Transaction;

namespace MoneyTracker.Queries.Domain.Repositories;
public interface IRegisterRepository
{
    public Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user);
}
