

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service.GivenARequestToSaveTransactionFromReceipt;
public class WhenEverythingIsValid : RegisterTestHelper, IAsyncLifetime
{
    private const string FileId = "file id of something";
    private const int PayeeId = 44;
    private const decimal Amount = 99m;
    private static readonly DateOnly DatePaid = new DateOnly(2025, 11, 10);
    private const int CategoryId = 67;
    private const int PayerId = 74;

    private const string ReceiptEntityId = "id of receipt entity";
    private const string ReceiptEntityName = "File name";
    private const string ReceiptEntityUrl = "Url of receipt";

    private const int PrevTransactionId = 101;
    private const int NextTransactionId = 102;

    private const string Token = "la token";
    private const int UserId = 24;

    private TransactionEntity _resultTransactionEntity;
    private ReceiptEntity _resultReceipt;
    private Result _result;

    public async Task InitializeAsync()
    {
        _mockUserService
            .Setup(x => x.GetUserFromToken(Token, CancellationToken.None))
            .ReturnsAsync(new AuthenticatedUser(UserId));

        _mockReceiptCommandRepository
            .Setup(x => x.GetReceiptById(FileId, CancellationToken.None))
            .ReturnsAsync(new ReceiptEntity(ReceiptEntityId, UserId, ReceiptEntityName, ReceiptEntityUrl, 4, null));

        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(PayerId, CancellationToken.None))
            .ReturnsAsync(new Domain.Entities.Account.AccountUserEntity(1, PayerId, UserId, true));
        _mockAccountDatabase
            .Setup(x => x.GetAccountUserEntity(PayeeId, CancellationToken.None))
            .ReturnsAsync(new Domain.Entities.Account.AccountUserEntity(1, PayeeId, UserId, false));

        _mockCategoryService
            .Setup(x => x.DoesCategoryExist(CategoryId, CancellationToken.None))
            .ReturnsAsync(true);

        _mockRegisterDatabase
            .Setup(x => x.GetLastTransactionId(CancellationToken.None))
            .ReturnsAsync(PrevTransactionId);

        _mockIdGenerator
            .Setup(x => x.NewInt(PrevTransactionId))
            .Returns(NextTransactionId);

        _mockRegisterDatabase
            .Setup(x => x.AddTransaction(It.IsAny<TransactionEntity>(), CancellationToken.None))
            .Callback((TransactionEntity entity, CancellationToken _) => _resultTransactionEntity = entity);

        _mockReceiptCommandRepository
            .Setup(x => x.UpdateReceipt(It.IsAny<ReceiptEntity>(), CancellationToken.None))
            .Callback((ReceiptEntity entity, CancellationToken _) => _resultReceipt = entity);

        _result = await _registerService.AddTransactionFromReceipt(Token,
            new NewTransactionFromReceiptRequest(FileId, PayeeId, Amount, DatePaid, CategoryId, PayerId),
            CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheTransactionEntityIsMappedCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(NextTransactionId, _resultTransactionEntity.Id);
            Assert.Equal(Amount, _resultTransactionEntity.Amount);
            Assert.Equal(DatePaid, _resultTransactionEntity.DatePaid);
            Assert.Equal(PayeeId, _resultTransactionEntity.PayeeId);
            Assert.Equal(PayerId, _resultTransactionEntity.PayerId);
            Assert.Equal(CategoryId, _resultTransactionEntity.CategoryId);
        });
    }

    [Fact]
    public void ThenTheReceiptEntityIsSetCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.Equal(ReceiptEntityId, _resultReceipt.Id);
            Assert.Equal(UserId, _resultReceipt.UserId);
            Assert.Equal(ReceiptEntityName, _resultReceipt.Name);
            Assert.Equal(ReceiptEntityUrl, _resultReceipt.Url);
            Assert.Equal(2, _resultReceipt.State);
            Assert.Equal(NextTransactionId, _resultReceipt.FinalTransactionId);
        });
    }
}
