
using MoneyTracker.Common.Result;

namespace MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas;
public interface IHandler : IVersionNumberObject
{
    Task<Result> Handle(string fileContents, string messageId, int userId, string filename, CancellationToken cancellationToken);
}
