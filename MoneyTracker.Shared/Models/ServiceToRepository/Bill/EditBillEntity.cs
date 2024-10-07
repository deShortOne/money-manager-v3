
namespace MoneyTracker.Shared.Models.ServiceToRepository.Bill;
public class EditBillEntity(int id, string? payee = null, decimal? amount = null, DateOnly? nextDueDate = null, string? frequency = null, int? category = null, int? accountId = null)
{
    public int Id { get; private set; } = id;
    public string? Payee { get; private set; } = payee;
    public decimal? Amount { get; private set; } = amount;
    public DateOnly? NextDueDate { get; private set; } = nextDueDate;
    public string? Frequency { get; private set; } = frequency;
    public int? Category { get; private set; } = category;
    public int? AccountId { get; } = accountId;

    public override bool Equals(object? obj)
    {
        var other = obj as EditBillEntity;
        if (other == null) return false;
        return Id == other.Id &&
            Payee == other.Payee &&
            Amount == other.Amount &&
            NextDueDate == other.NextDueDate &&
            Frequency == other.Frequency &&
            Category == other.Category &&
            AccountId == other.AccountId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Payee, Payee, Amount, NextDueDate, Frequency, Category, AccountId);
    }
}
