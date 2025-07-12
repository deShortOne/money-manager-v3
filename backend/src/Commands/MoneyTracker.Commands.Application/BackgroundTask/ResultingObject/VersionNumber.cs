namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject;

public interface IVersionNumberObject
{
    public int VersionNumber { get; }
}


public class VersionNumberObject : IVersionNumberObject
{
    public int VersionNumber { get; set; }
}
