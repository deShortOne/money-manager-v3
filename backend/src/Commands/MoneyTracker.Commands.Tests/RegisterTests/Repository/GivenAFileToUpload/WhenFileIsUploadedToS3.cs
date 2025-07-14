
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using MoneyTracker.Commands.Infrastructure.AWS;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenAFileToUpload;
public class WhenFileIsUploadedToS3 : IAsyncLifetime
{
    private Mock<IAmazonS3> _mockClient = new Mock<IAmazonS3>();
    private string _bucketName = "da bucket name";

    private Mock<IFormFile> _mockFile = new Mock<IFormFile>();
    private string _fileContentType = "file content type";
    private string _content = "da content from da file";
    private MemoryStream _inputStream;

    private string _id = "B7912A0D-9ADA-4C62-BDF0-F10E1E10E352";

    private PutObjectRequest _resultRequest;
    private string _resultUrl;

    public async Task InitializeAsync()
    {
        _mockClient.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Callback((PutObjectRequest por, CancellationToken _) => _resultRequest = por);

        _mockFile.Setup(x => x.ContentType)
            .Returns(_fileContentType);
        _inputStream = new MemoryStream();
        var writer = new StreamWriter(_inputStream);
        writer.Write(_content);
        writer.Flush();
        _inputStream.Position = 0;
        _mockFile.Setup(x => x.OpenReadStream())
            .Returns(_inputStream);

        var sut = new S3Repository(_mockClient.Object, _bucketName);

        _resultUrl = await sut.UploadAsync(_mockFile.Object, _id, CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public void ThenTheUrlReturnedIsCorrect()
    {
        Assert.Equal("https://da bucket name.s3.amazonaws.com/B7912A0D-9ADA-4C62-BDF0-F10E1E10E352", _resultUrl);
    }

    [Fact]
    public void ThenTheUploadOnlyHappensOnce()
    {
        _mockClient.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ThenThePutObjectRequestIsCorrect()
    {
        Assert.Equal(_id, _resultRequest.Key);
        Assert.Equal(_bucketName, _resultRequest.BucketName);
        Assert.Equal(_fileContentType, _resultRequest.ContentType);
        Assert.Equal(_inputStream, _resultRequest.InputStream);
    }
}
