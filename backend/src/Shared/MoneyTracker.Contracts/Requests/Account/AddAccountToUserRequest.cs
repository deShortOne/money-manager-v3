namespace MoneyTracker.Contracts.Requests.Account;
public class AddAccountToUserRequest(string accountname, bool doesUserOwnAccount)
{
    public string AccountName { get; } = accountname;
    public bool DoesUserOwnAccount { get; } = doesUserOwnAccount;
}
