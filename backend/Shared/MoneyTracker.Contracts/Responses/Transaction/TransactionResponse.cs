
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Contracts.Responses.Transaction;
public class TransactionResponse(
    int id,
    AccountResponse payee,
    decimal amount,
    DateOnly datePaid,
    CategoryResponse category,
    AccountResponse payer)
{

    public int Id { get; } = id;
    public AccountResponse Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly DatePaid { get; } = datePaid;
    public CategoryResponse Category { get; } = category;
    public AccountResponse Payer { get; } = payer;

    public override bool Equals(object? obj)
    {
        var other = obj as TransactionResponse;
        if (other == null)
            return false;

        return Id == other.Id
            && Payee.Equals(other.Payee)
            && Amount == other.Amount
            && DatePaid == other.DatePaid
            && Category.Equals(other.Category)
            && Payer.Equals(other.Payer);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Payee, Amount, DatePaid, Category, Payer);
    }
}
