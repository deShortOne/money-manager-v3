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
    public int Id { get; } = id;
    public AccountResponse Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public CategoryResponse Category { get; } = category;
    public OverDueBillInfo? OverDueBill { get; } = overDueBill;
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
