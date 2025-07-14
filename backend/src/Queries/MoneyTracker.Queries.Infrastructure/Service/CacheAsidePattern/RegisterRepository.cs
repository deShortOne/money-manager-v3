
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using MoneyTracker.Queries.Domain.Entities.Transaction;
using MoneyTracker.Queries.Domain.Repositories.Cache;
using MoneyTracker.Queries.Domain.Repositories.Database;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Infrastructure.Service.CacheAsidePattern;
public class RegisterRepository : IRegisterRepositoryService
{
    private readonly IRegisterDatabase _registerDatabase;
    private readonly IRegisterCache _registerCache;

    public RegisterRepository(
        IRegisterDatabase registerDatabase,
        IRegisterCache registerCache
        )
    {
        _registerDatabase = registerDatabase;
        _registerCache = registerCache;
    }

    public async Task<ResultT<List<TransactionEntity>>> GetAllTransactions(AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        var result = await _registerCache.GetAllTransactions(user, cancellationToken);
        if (result.HasError)
        {
            result = await _registerDatabase.GetAllTransactions(user, cancellationToken);
            await _registerCache.SaveTransactions(user, result.Value, cancellationToken);
        }

        return result;
    }

    public Task<ResultT<ReceiptEntity>> GetReceiptProcessingInfo(string receiptId, CancellationToken cancellationToken)
    {
        return _registerDatabase.GetReceiptProcessingInfo(receiptId, cancellationToken);
    }

    public Task<ResultT<TemporaryTransaction>> GetTemporaryTransactionFromReceipt(string fileId, CancellationToken cancellationToken)
    {
        return _registerDatabase.GetTemporaryTransactionFromReceipt(fileId, cancellationToken);
    }

    public async Task ResetTransactionsCache(AuthenticatedUser user, CancellationToken cancellationToken)
    {
        var result = await _registerDatabase.GetAllTransactions(user, cancellationToken);
        await _registerCache.SaveTransactions(user, result.Value, cancellationToken);
    }

    public Task<List<ReceiptIdAndStateEntity>> GetReceiptStatesForUser(AuthenticatedUser user, List<ReceiptState> designatedStates, CancellationToken cancellationToken)
    {
        return _registerDatabase.GetReceiptStatesForUser(user, designatedStates, cancellationToken);
    }
}
