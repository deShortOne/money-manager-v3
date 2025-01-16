
namespace MoneyTracker.Common.Utilities.IdGeneratorUtil;
public class IdGenerator : IIdGenerator
{
    public Guid NewGuid => Guid.NewGuid();
    public int NewInt(int prevId) => prevId + 1;
}
