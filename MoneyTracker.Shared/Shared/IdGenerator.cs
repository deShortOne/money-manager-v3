
namespace MoneyTracker.Shared.Shared;

public interface IIdGenerator
{
    Guid NewGuid { get; }
    int NewInt(int prevId);
}

public class IdGenerator : IIdGenerator
{
    public Guid NewGuid => Guid.NewGuid();
    public int NewInt(int prevId) => prevId + 1;
}
