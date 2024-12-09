
using MoneyTracker.Authentication.DTOs;
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
    private readonly string _payee = "Specsavers";
    private readonly decimal _amount = 25;
    private readonly DateOnly _datePaid = new DateOnly(2024, 12, 8);
    private readonly int _categoryId = 2;
    private readonly int _accountId = 15;

    public AddRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    [Fact]
    public async void SuccessfullyAddNewTransaction()
    {
        _mockUserAuthService.Setup(x => x.DecodeToken(_tokenToDecode)).Returns(Task.FromResult(_authedUser));
        _mockRegisterDatabase.Setup(x => x.GetLastTransactionId()).Returns(Task.FromResult(_lastTransactionId));
        _mockIdGenerator.Setup(x => x.NewInt(_lastTransactionId)).Returns(_newTransactionId);
        _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, _accountId)).Returns(Task.FromResult(true));

        var newTransactionRequest = new NewTransactionRequest(_payee, _amount, _datePaid, _categoryId, _accountId);
        var newTransaction = new TransactionEntity(_newTransactionId, _payee, _amount, _datePaid, _categoryId, _accountId);

        await _registerService.AddTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetLastTransactionId(), Times.Once);
            _mockRegisterDatabase.Verify(x => x.AddTransaction(newTransaction), Times.Once);
            _mockIdGenerator.Verify(x => x.NewInt(_lastTransactionId), Times.Once);
            _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, _accountId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
