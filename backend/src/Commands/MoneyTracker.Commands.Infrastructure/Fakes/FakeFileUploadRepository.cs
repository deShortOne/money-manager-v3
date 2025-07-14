
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V2;
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
            VersionNumber = 2,
            Data = new Data
            {
                // for user root only
                DatePaid = new DateOnly(2025, 7, 12),
                PayeeName = "Bank A",
                PayerName = "Supermarket A",
                Amount = 6.4m,
            },
        };
        return JsonSerializer.Serialize(res);
    }
}
