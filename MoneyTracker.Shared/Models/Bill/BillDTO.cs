
namespace MoneyTracker.Shared.Models.Bill;
public class BillDTO(int id, string payee, decimal amount, DateOnly nextDueDate, string frequency, string category)
{
    public int Id { get; private set; } = id;
    public string Payee { get; private set; } = payee;
    public decimal Amount { get; private set; } = amount;
    public DateOnly NextDueDate { get; private set; } = nextDueDate;
    public string Frequency { get; private set; } = frequency;
    public string Category { get; private set; } = category;

    public override bool Equals(System.Object obj)
    {
        var other = obj as BillDTO;

        if (other == null)
        {
            return false;
        }

        // Instances are considered equal if the ReferenceId matches.
        return Payee == other.Payee && Amount == other.Amount && NextDueDate == other.NextDueDate && Frequency == other.Frequency && Category == other.Category;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
