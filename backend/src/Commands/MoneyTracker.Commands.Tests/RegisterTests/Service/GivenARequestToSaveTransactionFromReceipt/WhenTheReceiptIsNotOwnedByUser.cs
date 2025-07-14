

using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Receipt;
using MoneyTracker.Common.Result;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service.GivenARequestToSaveTransactionFromReceipt;
public class WhenTheReceiptIsNotOwnedByUser : RegisterTestHelper, IAsyncLifetime
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

    private const string Token = "la token";
    private const int UserId = 24;

    private Result _result;

    public async Task InitializeAsync()
    {
        _mockUserService
            .Setup(x => x.GetUserFromToken(Token, CancellationToken.None))
            .ReturnsAsync(new AuthenticatedUser(UserId));

        _mockReceiptCommandRepository
            .Setup(x => x.GetReceiptById(FileId, CancellationToken.None))
            .ReturnsAsync(new ReceiptEntity(ReceiptEntityId, UserId + 1, ReceiptEntityName, ReceiptEntityUrl, 4, null));

        _result = await _registerService.AddTransactionFromReceipt(Token,
            new NewTransactionFromReceiptRequest(FileId, PayeeId, Amount, DatePaid, CategoryId, PayerId),
            CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.HasError);
    }

    [Fact]
    public void ThenTheErroMessageIsCorrect()
    {
        Assert.Equal(ErrorType.NotFound, _result.Error!.ErrorType);
        Assert.Equal("RegisterService.AddTransactionFromReceipt", _result.Error!.Code);
        Assert.Equal($"Cannot find entity: {FileId} for user {UserId}", _result.Error!.Description);
    }
}
