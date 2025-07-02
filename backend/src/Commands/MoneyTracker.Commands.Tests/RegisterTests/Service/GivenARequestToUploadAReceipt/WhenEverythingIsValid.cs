
using Microsoft.AspNetCore.Http;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service.GivenARequestToUploadAReceipt;
public class WhenEverythingIsValid : RegisterTestHelper, IAsyncLifetime
{
    private string _token = "71AE322B-6A27-46BC-A3FE-577BFE78CB0C";
    private int _userId = 2;
    private string _fileName = "receipt.AA.jpg";
    private string _fileUploadId = "receipt.AA-20250629-142455.jpg";
    private string _fileUploadUrl = "a-url-goes-here";

    private Mock<IFormFile> _mockFileFile = new Mock<IFormFile>();

    private ReceiptEntity _resultReceiptEntity;
    private Result _result;

    public async Task InitializeAsync()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_token, CancellationToken.None))
            .ReturnsAsync(new AuthenticatedUser(_userId));

        _mockDateTimeProvider.Setup(x => x.Now)
            .Returns(new DateTime(2025, 6, 29, 14, 24, 55));

        _mockFileFile.Setup(x => x.FileName)
            .Returns(_fileName);

        _mockFileUploadRepository.Setup(x => x.UploadAsync(_mockFileFile.Object, _fileUploadId, CancellationToken.None))
            .ReturnsAsync(_fileUploadUrl);

        _mockReceiptCommandRepository.Setup(x => x.AddReceipt(It.IsAny<ReceiptEntity>(), CancellationToken.None))
            .Callback((ReceiptEntity entity, CancellationToken _) => _resultReceiptEntity = entity);

        _result = await _registerService.CreateTransactionFromReceipt(_token, _mockFileFile.Object, CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenRequestToGetUserIsOnlyCalledOnce()
    {
        _mockUserService.Verify(x => x.GetUserFromToken(_token, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheFileIsUploadedOnlyOnce()
    {
        _mockFileUploadRepository.Verify(x => x.UploadAsync(_mockFileFile.Object, _fileUploadId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheReceiptStateIsUploadedOnceToRepository()
    {
        _mockReceiptCommandRepository.Verify(x => x.AddReceipt(It.IsAny<ReceiptEntity>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheReceiptStateIsCorrect()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(_fileUploadId, _resultReceiptEntity.Id);
            Assert.Equal(_userId, _resultReceiptEntity.UserId);
            Assert.Equal(_fileName, _resultReceiptEntity.Name);
            Assert.Equal(_fileUploadUrl, _resultReceiptEntity.Url);
            Assert.Equal(1, _resultReceiptEntity.State);
        });
    }
}
