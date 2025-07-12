
using System.Text.Json;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V1;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestForMessageQueueServiceToPoll.V1;
public class WhenEverythingIsValid : MessageQueueServiceHelper
{
    private readonly string _successfulMessageMessageId = "EFDF6BA9-4D8F-45C9-A4B2-B08265EAF211";
    private readonly string _successfulMessageFilename = "3C8543DD-8F8B-41B4-8A6C-0067E31C719C";
    private readonly string _successfulMessageQueueMessageId = "EFDF6BA9-4D8F-45C9-A4B2-B08265EAF211";

    private readonly string _receiptId = "BB90E2CF-8C90-4C2F-833F-6BD8A10DB96F";
    private readonly int _userId = 125;
    private readonly string _receiptName = "50A0D88C-1BA2-418C-B286-DC33FE0E5F42";
    private readonly string _receiptFileUrl = "4B5EC901-CE16-4EE3-9502-461BEA2A72E7";
    private readonly decimal _amount = 608.2m;
    private readonly int _previousReceiptState = 1;
    private readonly int _nextReceiptState = 4;

    private TemporaryTransactionEntity _resultTemporaryTransaction;
    private ReceiptEntity _resultReceiptEntity;
    private ResultT<MessageQueueResult> _result;

    public override async Task InitializeAsync()
    {
        _mockMessageQueueRepository
            .Setup(x => x.GetFileNamesThatHaveBeenProcessed(CancellationToken.None))
            .ReturnsAsync(new SuccessfulFileNamesAndFailedMessageIds
            {
                SuccessfulFiles = [
                    new SuccessfulMessageInfo
                    {
                        MessageId = _successfulMessageMessageId,
                        Filename = _successfulMessageFilename,
                        QueueMessageId = _successfulMessageQueueMessageId
                    },
                ],
                FailedMessageIds = []
            });

        _mockReceiptCommandRepository
            .Setup(x => x.GetReceiptById(_successfulMessageFilename, CancellationToken.None))
            .ReturnsAsync(new ReceiptEntity(_receiptId, _userId, _receiptName, _receiptFileUrl, _previousReceiptState));

        var fileContentsFromFileUploadRepository = new TemporaryTransactionObject
        {
            VersionNumber = 1,
            Data = new Data
            {
                Value = _amount
            },
        };
        _mockFileUploadRepository
            .Setup(x => x.GetContentsOfFile(_successfulMessageFilename, CancellationToken.None))
            .ReturnsAsync(JsonSerializer.Serialize(fileContentsFromFileUploadRepository));

        _mockReceiptCommandRepository
            .Setup(x => x.CreateTemporaryTransaction(It.IsAny<TemporaryTransactionEntity>(), CancellationToken.None))
            .Callback((TemporaryTransactionEntity temporaryTransactionEntity, CancellationToken _) => _resultTemporaryTransaction = temporaryTransactionEntity);

        _mockReceiptCommandRepository
            .Setup(x => x.UpdateReceipt(It.IsAny<ReceiptEntity>(), CancellationToken.None))
            .Callback((ReceiptEntity receiptEntity, CancellationToken _) => _resultReceiptEntity = receiptEntity);

        _mockReceiptCommandRepository
            .Setup(x => x.GetNumberOfReceiptsLeftToProcess(CancellationToken.None))
            .ReturnsAsync(0);

        _result = await _messageQueueService.PollAsync(CancellationToken.None);
    }

    public override Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheSuccessfullyProcessedFileIdsAreMappedCorrectly()
    {
        var successLis = _result.Value.SuccessfullyProcessedFileIds;
        Assert.Single(successLis);
        Assert.Equal(_successfulMessageMessageId, successLis[0]);
    }

    [Fact]
    public void ThenTheFailedProcessedFileIdsAreMappedCorrectly()
    {
        Assert.Empty(_result.Value.FailedProcessedFileIds);
    }

    [Fact]
    public void ThenTheReceiptIsRequestedOnce()
    {
        _mockReceiptCommandRepository.Verify(x => x.GetReceiptById(_successfulMessageFilename, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheFileIsDownloadedOnce()
    {
        _mockFileUploadRepository.Verify(x => x.GetContentsOfFile(_successfulMessageFilename, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheTemporaryTransactionIsCreatedOnce()
    {
        _mockReceiptCommandRepository.Verify(x => x.CreateTemporaryTransaction(It.IsAny<TemporaryTransactionEntity>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheTemporaryTransactionIsMappedCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(_userId, _resultTemporaryTransaction.UserId);
            Assert.Equal(_amount, _resultTemporaryTransaction.Amount);
            Assert.Null(_resultTemporaryTransaction.CategoryId);
            Assert.Null(_resultTemporaryTransaction.DatePaid);
            Assert.Null(_resultTemporaryTransaction.PayeeId);
            Assert.Null(_resultTemporaryTransaction.PayerId);
        });
    }

    [Fact]
    public void ThenTheCallToUpdateTheReceiptIsCalledOnce()
    {
        _mockReceiptCommandRepository.Verify(x => x.UpdateReceipt(It.IsAny<ReceiptEntity>(), CancellationToken.None), Times.Once);
    }

    [Fact]
    public void ThenTheReceiptEntityIsMappedCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(_receiptId, _resultReceiptEntity.Id);
            Assert.Equal(_userId, _resultReceiptEntity.UserId);
            Assert.Equal(_receiptName, _resultReceiptEntity.Name);
            Assert.Equal(_receiptFileUrl, _resultReceiptEntity.Url);
            Assert.Equal(_nextReceiptState, _resultReceiptEntity.State);
        });
    }

    [Fact]
    public void ThenTheMessageIsDeletedOnce()
    {
        _mockMessageQueueRepository.Verify(x => x.DeleteMessage(_successfulMessageMessageId, CancellationToken.None), Times.Once);
    }

    [Fact]
    public void GivenThereAre0ReceiptsLeftToProcessThenThePollingControllerIsDisabled()
    {
        _mockPollingController.Verify(x => x.DisablePolling(), Times.Once);
        _mockPollingController.VerifyNoOtherCalls();
    }
}
