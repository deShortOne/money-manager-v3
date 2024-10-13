
namespace MoneyTracker.Common.Utilities.IdGeneratorUtil;
public interface IIdGenerator
{
    Guid NewGuid { get; }

    int NewInt(int prevId);
}
