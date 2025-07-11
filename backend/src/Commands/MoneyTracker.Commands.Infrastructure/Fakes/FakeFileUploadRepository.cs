
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MoneyTracker.Commands.Application.AWS;
using MoneyTracker.Commands.Domain.Repositories;

namespace MoneyTracker.Commands.Application.Fake;
public class FakeFileUploadRepository : IFileUploadRepository
{

    public async Task<string> UploadAsync(IFormFile file, string id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        SomethingInTheMiddle.LastFileId = id;

        return "da url";
    }

    public async Task<string> GetContentsOfFile(string id, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var res = new TemporaryTransactionObject
        {
            Value = 24,
        };
        return JsonSerializer.Serialize(res);
    }
}
