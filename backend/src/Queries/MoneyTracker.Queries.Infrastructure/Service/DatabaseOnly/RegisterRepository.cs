
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.DatabaseOnly;
public class RegisterRepository : IRegisterRepositoryService
{
    private readonly IRegisterDatabase _registerDatabase;

    public RegisterRepository(IRegisterDatabase registerDatabase)
    {
        _registerDatabase = registerDatabase;
    }

    public Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        return _registerDatabase.GetAllTransactions(user, cancellationToken);
    }

    public Task ResetTransactionsCache(AuthenticatedUser user, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<ResultT<ReceiptEntity>> GetReceiptProcessingInfo(string receiptId, CancellationToken cancellationToken)
    {
        return _registerDatabase.GetReceiptProcessingInfo(receiptId, cancellationToken);
    }
}
