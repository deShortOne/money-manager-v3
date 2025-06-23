using System.Text.Json.Serialization;
using MoneyTracker.Common.DTOs;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Contracts.Responses.Bill;
public class BillResponse(int id,
    AccountResponse payee,
    decimal amount,
    DateOnly nextDueDate,
    string frequency,
    CategoryResponse category,
    OverDueBillInfo? overDueBill,
    AccountResponse payer
    )
{
    [JsonPropertyName("id")]
    public int Id { get; } = id;

    [JsonPropertyName("payee")]
    public AccountResponse Payee { get; } = payee;

    [JsonPropertyName("amount")]
    public decimal Amount { get; } = amount;

    [JsonPropertyName("nextduedate")]
    public DateOnly NextDueDate { get; } = nextDueDate;

    [JsonPropertyName("frequency")]
    public string Frequency { get; } = frequency;

    [JsonPropertyName("category")]
    public CategoryResponse Category { get; } = category;

    [JsonPropertyName("overduebill")]
    public OverDueBillInfo? OverDueBill { get; } = overDueBill;

    [JsonPropertyName("payer")]
    public AccountResponse Payer { get; } = payer;

    public override bool Equals(object? obj)
    {
        var other = obj as BillResponse;

        if (other == null)
        {
            return false;
        }

        if ((OverDueBill == null) != (other.OverDueBill == null))
        {
            return false;
        }

        bool isOverDueBillSame = OverDueBill == null;
        if (OverDueBill != null)
        {
            isOverDueBillSame = OverDueBill.Equals(other.OverDueBill);
        }

        return Id == other.Id
            && Payee.Equals(other.Payee)
            && Amount == other.Amount
            && NextDueDate == other.NextDueDate
            && Frequency == other.Frequency
            && Category.Equals(other.Category)
            && isOverDueBillSame
            && Payer.Equals(other.Payer);
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Id);
        hash.Add(Payee);
        hash.Add(Amount);
        hash.Add(NextDueDate);
        hash.Add(Frequency);
        hash.Add(Category);
        hash.Add(OverDueBill);
        hash.Add(Payer);
        return hash.ToHashCode();
    }
}
