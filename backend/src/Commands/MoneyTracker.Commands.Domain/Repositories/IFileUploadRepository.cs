
using Microsoft.AspNetCore.Http;

namespace MoneyTracker.Commands.Domain.Repositories;
public interface IFileUploadRepository
{
    Task<string> UploadAsync(IFormFile file, string id);
    Task<string> GetContentsOfFile(string id, CancellationToken ct);
}
