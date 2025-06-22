using System.Diagnostics.CodeAnalysis;

namespace MoneyTracker.Common.Utilities.IdGeneratorUtil;
[ExcludeFromCodeCoverage]
public class IdGenerator : IIdGenerator
{
    public Guid NewGuid => Guid.NewGuid();
    public int NewInt(int prevId) => prevId + 1;
}
