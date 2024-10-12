
namespace MoneyTracker.Shared.Models.ServiceToRepository.Bill;
public class DeleteBillDTO(int id)
{
    public int Id { get; private set; } = id;

    public override bool Equals(object? obj)
    {
        var other = obj as DeleteBillDTO;
        if (other == null) return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
