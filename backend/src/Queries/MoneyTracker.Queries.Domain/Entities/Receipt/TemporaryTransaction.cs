
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Queries.Domain.Entities.Receipt;
public class TemporaryTransaction(
    int userId,
    string filename,
    AccountResponse? payee,
    decimal? amount,
    DateOnly? datePaid,
    CategoryResponse? category,
    AccountResponse? payer)
{
    public int UserId { get; } = userId;
    public string Filename { get; } = filename;
    public AccountResponse? Payee { get; } = payee;
    public decimal? Amount { get; } = amount;
    public DateOnly? DatePaid { get; } = datePaid;
    public CategoryResponse? Category { get; } = category;
    public AccountResponse? Payer { get; } = payer;
}
