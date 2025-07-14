
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Values;
using MoneyTracker.Contracts.Responses.Account;
using MoneyTracker.Contracts.Responses.Category;
using MoneyTracker.Contracts.Responses.Receipt;
using MoneyTracker.Queries.Domain.Entities.Receipt;
using Moq;

namespace MoneyTracker.Queries.Tests.RegisterTests.Service.GivenARequestToGetTransactionFromReceipt;
public class WhenTheReceiptStateIsPending : RegisterTestHelper, IAsyncLifetime
{
    private readonly string _token = "da token";
    private readonly string _filename = "da file name";
    private readonly int _userId = 24;
    private readonly AccountResponse _payee = new AccountResponse(24, "payee name");
    private readonly decimal _amount = 524m;
    private readonly DateOnly _datePaid = new DateOnly(2025, 7, 7);
    private readonly CategoryResponse _categoryResponse = new CategoryResponse(64, "category 1");
    private readonly AccountResponse _payer = new AccountResponse(25, "payer name a");

    private ResultT<ReceiptResponse> _result;

    public async Task InitializeAsync()
    {
        var userAuthentication = new Mock<IUserAuthentication>();

        _mockUserRepository
            .Setup(x => x.GetUserAuthFromToken(_token, CancellationToken.None))
            .ReturnsAsync(userAuthentication.Object);

        _mockRegisterDatabase
            .Setup(x => x.GetReceiptProcessingInfo(_filename, CancellationToken.None))
            .ReturnsAsync(new ReceiptEntity("", -1, _filename, "", ReceiptState.Pending));

        _mockRegisterDatabase
            .Setup(x => x.GetTemporaryTransactionFromReceipt(_filename, CancellationToken.None))
            .ReturnsAsync(new TemporaryTransaction(_userId, _filename, _payee, _amount, _datePaid, _categoryResponse, _payer));

        _result = await _registerService.GetTransactionFromReceipt(_token, _filename, CancellationToken.None);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public void ThenThereAreNoErrors()
    {
        Assert.True(_result.IsSuccess);
    }

    [Fact]
    public void ThenTheStateOnTheResponseObjectIsMappedCorrectly()
    {
        Assert.Equal("Pending", _result.Value.ReceiptProcessingState);
    }

    [Fact]
    public void ThenTheTemporaryReceiptObjectIsMappedCorrectly()
    {
        var temporaryTransaction = _result.Value.TemporaryTransaction;
        Assert.Multiple(() =>
        {
            Assert.NotNull(temporaryTransaction);
            Assert.Equal(_payee, temporaryTransaction.Payee);
            Assert.Equal(_amount, temporaryTransaction.Amount);
            Assert.Equal(_datePaid, temporaryTransaction.DatePaid);
            Assert.Equal(_categoryResponse, temporaryTransaction.Category);
            Assert.Equal(_payer, temporaryTransaction.Payer);
        });
    }
}
