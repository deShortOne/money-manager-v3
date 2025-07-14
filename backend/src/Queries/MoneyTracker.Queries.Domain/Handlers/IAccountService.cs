using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Responses.Account;

namespace MoneyTracker.Queries.Domain.Handlers;
public interface IAccountService
{
    Task<ResultT<List<AccountResponse>>> GetAccounts(string token, CancellationToken cancellationToken);
}
