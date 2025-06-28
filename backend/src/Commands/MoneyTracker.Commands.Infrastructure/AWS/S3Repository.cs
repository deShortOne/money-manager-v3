
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using MoneyTracker.Commands.Domain.Repositories;

namespace MoneyTracker.Commands.Infrastructure.AWS;
public class S3Repository : IFileUploadRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Repository(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task<string> UploadAsync(IFormFile file, string id)
    {
        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            Key = id,
            BucketName = _bucketName,
            ContentType = file.ContentType,
            InputStream = stream,
        };

        await _s3Client.PutObjectAsync(request);

        return $"https://{_bucketName}.s3.amazonaws.com/{id}";
    }
}
