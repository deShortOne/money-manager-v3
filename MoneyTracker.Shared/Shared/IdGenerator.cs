
namespace MoneyTracker.Shared.Shared;

public interface IIdGenerator
{
    Guid NewGuid { get; }
}

public class IdGenerator : IIdGenerator
{
    public Guid NewGuid => Guid.NewGuid();
}
