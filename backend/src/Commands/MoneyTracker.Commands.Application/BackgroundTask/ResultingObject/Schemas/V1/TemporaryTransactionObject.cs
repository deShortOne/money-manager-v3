namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V1;
public class TemporaryTransactionObject : VersionNumberObject
{
    public int VersionNumber { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public decimal Value { get; set; }
}
