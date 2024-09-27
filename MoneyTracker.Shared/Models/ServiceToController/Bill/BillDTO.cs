
namespace MoneyTracker.Shared.Models.ServiceToController.Bill;
public class BillResponseDTO(int id, string payee, decimal amount, DateOnly nextDueDate,
    string frequency, string category, OverDueBillInfo? overDueBill, string accountName)
{
    public int Id { get; private set; } = id;
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public string Category { get; private set; } = category;
    public OverDueBillInfo? OverDueBill { get; private set; } = overDueBill;
    public string AccountName { get; } = accountName;

    public override bool Equals(object? obj)
    {
        var other = obj as BillResponseDTO;

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
