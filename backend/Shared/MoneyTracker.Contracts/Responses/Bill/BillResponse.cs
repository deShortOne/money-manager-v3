using MoneyTracker.Common.DTOs;

namespace MoneyTracker.Contracts.Responses.Bill;
public class BillResponse(int id, string payee, decimal amount, DateOnly nextDueDate,
    string frequency, string category, OverDueBillInfo? overDueBill, string accountName)
{
    public int Id { get; } = id;
    public string Payee { get; } = payee;
    public decimal Amount { get; } = amount;
    public DateOnly NextDueDate { get; } = nextDueDate;
    public string Frequency { get; } = frequency;
    public string Category { get; } = category;
    public OverDueBillInfo? OverDueBill { get; } = overDueBill;
    public string AccountName { get; } = accountName;

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

        return Id == other.Id && Payee == other.Payee && Amount == other.Amount &&
            NextDueDate == other.NextDueDate && Frequency == other.Frequency &&
            Category == other.Category && isOverDueBillSame && AccountName == other.AccountName;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
