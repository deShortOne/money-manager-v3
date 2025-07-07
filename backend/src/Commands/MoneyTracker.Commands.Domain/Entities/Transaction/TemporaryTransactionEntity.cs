
namespace MoneyTracker.Commands.Domain.Entities.Transaction;
public class TemporaryTransactionEntity()
{
    public int UserId { get; set; }
    public required string Filename { get; set; }
    public int? PayeeId { get; set; }
    public decimal? Amount { get; set; }
    public DateOnly? DatePaid { get; set; }
    public int? CategoryId { get; set; }
    public int? PayerId { get; set; }
}
