namespace MoneyTracker.Commands.Domain.Entities.Account;
public class AccountUserEntity(int id, int accountId, int userId, bool userOwnsAccount)
{
    public int Id { get; } = id;
    public int AccountId { get; } = accountId;
    public int UserId { get; } = userId;
    public bool UserOwnsAccount { get; } = userOwnsAccount;

    public override bool Equals(object? obj)
    {
        var other = obj as AccountUserEntity;
        if (other == null) return false;
        return Id == other.Id
            && AccountId == other.AccountId
            && UserId == other.UserId
            && UserOwnsAccount == other.UserOwnsAccount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, AccountId, UserId, UserOwnsAccount);
    }
}
