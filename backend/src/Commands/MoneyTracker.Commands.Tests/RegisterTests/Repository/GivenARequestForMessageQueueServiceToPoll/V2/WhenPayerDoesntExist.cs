
using System.Text.Json;
using MoneyTracker.Commands.Application.BackgroundTask.ResultingObject.Schemas.V2;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.MessageQueuePolling;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Result;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Repository.GivenARequestForMessageQueueServiceToPoll.V2;
public class WhenPayerDoesntExist : MessageQueueServiceHelper
{
    private readonly string _successfulMessageMessageId = "EFDF6BA9-4D8F-45C9-A4B2-B08265EAF211";
    private readonly string _successfulMessageFilename = "3C8543DD-8F8B-41B4-8A6C-0067E31C719C";
    private readonly string _successfulMessageQueueMessageId = "EFDF6BA9-4D8F-45C9-A4B2-B08265EAF211";

    private readonly string _receiptId = "BB90E2CF-8C90-4C2F-833F-6BD8A10DB96F";
    private readonly int _userId = 125;
    private readonly string _receiptName = "50A0D88C-1BA2-418C-B286-DC33FE0E5F42";
    private readonly string _receiptFileUrl = "4B5EC901-CE16-4EE3-9502-461BEA2A72E7";
    private readonly DateOnly _datePaid = new DateOnly(2025, 7, 12);
    private readonly decimal _amount = 608.2m;
    private readonly string _payee = "payee name";
    private readonly string _payer = "payer namee";
    private readonly int _previousReceiptState = 1;
    private readonly int _nextReceiptState = 4;

    private readonly int _payeeId = 62;
    private readonly AccountUserEntity _payeeAccountUserEntity;

    private ResultT<MessageQueueResult> _result;

    public WhenPayerDoesntExist()
    {
        _payeeAccountUserEntity = new AccountUserEntity(_payeeId, 2894, _userId, true);
    }

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
            VersionNumber = 2,
            Data = new Data
            {
                DatePaid = _datePaid,
                Amount = _amount,
                PayeeName = _payee,
                PayerName = _payer,
            },
        };
        _mockFileUploadRepository
            .Setup(x => x.GetContentsOfFile(_successfulMessageFilename, CancellationToken.None))
            .ReturnsAsync(JsonSerializer.Serialize(fileContentsFromFileUploadRepository));

        _mockReceiptCommandRepository
            .Setup(x => x.GetNumberOfReceiptsLeftToProcess(CancellationToken.None))
            .ReturnsAsync(0);

        _mockAccountCommandRepository
            .Setup(x => x.GetAccountUserEntity(_payee, _userId, CancellationToken.None))
            .ReturnsAsync(_payeeAccountUserEntity);

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
        Assert.Empty(_result.Value.SuccessfullyProcessedFileIds);
    }

    [Fact]
    public void ThenTheFailedProcessedFileIdsAreMappedCorrectly()
    {
        Assert.Single(_result.Value.FailedProcessedFileIds);

        var result = _result.Value.FailedProcessedFileIds[0];
        Assert.Multiple(() =>
        {
            Assert.True(result.HasError);
            Assert.Equal($"ERROR: payer cannot be found: {_payer}", result.Error!.Description);
        });
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
    public void ThenTheTemporaryTransactionIsNeverCreated()
    {
        _mockReceiptCommandRepository.Verify(x => x.CreateTemporaryTransaction(It.IsAny<TemporaryTransactionEntity>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public void ThenTheCallToUpdateTheReceiptIsNeverCalled()
    {
        _mockReceiptCommandRepository.Verify(x => x.UpdateReceipt(It.IsAny<ReceiptEntity?>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public void ThenTheMessageIsNeverDeleted()
    {
        _mockMessageQueueRepository.Verify(x => x.DeleteMessage(It.IsAny<string>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public void GivenThereAre0ReceiptsLeftToProcessThenThePollingControllerIsDisabled()
    {
        _mockPollingController.Verify(x => x.DisablePolling(), Times.Once);
        _mockPollingController.VerifyNoOtherCalls();
    }
}
