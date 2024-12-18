using MoneyTracker.Authentication.DTOs;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IAccountCommandRepository
{
    Task<bool> IsAccountOwnedByUser(AuthenticatedUser user, int accountId);
}
