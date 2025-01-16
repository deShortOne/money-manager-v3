
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public sealed class AddRegisterTest : RegisterTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _lastTransactionId = 2;
    private readonly int _newTransactionId = 1;
    private readonly int _payeeId = 87;
    private readonly decimal _amount = 25;
    private readonly DateOnly _datePaid = new DateOnly(2024, 12, 8);
    private readonly int _categoryId = 2;
    private readonly int _payerId = 15;

    public AddRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    [Fact]
    public async void SuccessfullyAddNewTransaction()
    {
        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase.Setup(x => x.GetAccountById(_payerId)).ReturnsAsync(new AccountEntity(1, "", _userId));

        var newTransactionRequest = new NewTransactionRequest(_payeeId, _amount, _datePaid, _categoryId, _payerId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payeeId, _amount, _datePaid, _categoryId, _payerId);

        await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetLastTransactionId(), Times.Once);
            _mockRegisterDatabase.Verify(x => x.AddTransaction(newTransaction), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastTransactionId), Times.Once);
            _mockAccountDatabase.Verify(x => x.GetAccountById(_payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
