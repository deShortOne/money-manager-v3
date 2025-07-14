using System.Text.Json.Serialization;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;

namespace MoneyTracker.Contracts.Responses.Receipt;
public class ReceiptResponse(string receiptProcessingState, TemporaryTransactionResponse? temporaryTransaction)
{
    [JsonPropertyName("receiptProcessingState")]
    public string ReceiptProcessingState { get; } = receiptProcessingState;
    [JsonPropertyName("temporaryTransaction")]
    public TemporaryTransactionResponse? TemporaryTransaction { get; } = temporaryTransaction;
}

public class TemporaryTransactionResponse(AccountResponse? payee,
    decimal? amount,
    DateOnly? datePaid,
    CategoryResponse? category,
    AccountResponse? payer)
{
    [JsonPropertyName("payee")]
    public AccountResponse? Payee { get; } = payee;

    [JsonPropertyName("amount")]
    public decimal? Amount { get; } = amount;

    [JsonPropertyName("datePaid")]
    public DateOnly? DatePaid { get; } = datePaid;

    [JsonPropertyName("category")]
    public CategoryResponse? Category { get; } = category;

    [JsonPropertyName("payer")]
    public AccountResponse? Payer { get; } = payer;
}

