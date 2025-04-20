namespace MoneyTracker.Contracts.Requests.Account;
public class AddAccountToUserRequest(int accountId, bool doesUserOwnAccount)
{
    public int AccountId { get; } = accountId;
    public bool DoesUserOwnAccount { get; } = doesUserOwnAccount;
}
