
using System.Text.Json;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas;
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject;
public class StrategyContext
{
    private readonly Dictionary<int, IHandler> _strategies;

    public StrategyContext(IEnumerable<IHandler> strategies)
    {

        _strategies = strategies.ToDictionary(s => s.VersionNumber);
    }

    public ResultT<IHandler?> GetStrategy(string fileContents)
    {
        var fileWithKey = JsonSerializer.Deserialize<VersionNumberObject>(fileContents);
        if (fileWithKey is null)
            return Error.NotFound("", $"Could not find version number in: {fileContents}");

        var strategy = _strategies[fileWithKey.VersionNumber];
        if (strategy is null)
            return Error.NotFound("", $"Could not find strategy with version number in: {fileContents}");

        return ResultT<IHandler?>.Success(strategy);
    }
}
