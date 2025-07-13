namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V2;
public class TemporaryTransactionObject : VersionNumberObject
{
    public int VersionNumber { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public decimal? Amount { get; set; }
    public DateOnly? DatePaid { get; set; }
    public string? PayeeName { get; set; }
    public string? PayerName { get; set; }
}
