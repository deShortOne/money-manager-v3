
using System.Text.Json.Serialization;
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

    [JsonPropertyName("id")]
    public int Id { get; } = id;

    [JsonPropertyName("payee")]
    public AccountResponse Payee { get; } = payee;

    [JsonPropertyName("amount")]
    public decimal Amount { get; } = amount;

    [JsonPropertyName("datePaid")]
    public DateOnly DatePaid { get; } = datePaid;

    [JsonPropertyName("category")]
    public CategoryResponse Category { get; } = category;

    [JsonPropertyName("payer")]
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
