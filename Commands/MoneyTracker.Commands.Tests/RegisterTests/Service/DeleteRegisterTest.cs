
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public sealed class DeleteRegisterTest : RegisterTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";
    
    private readonly int _transactionId = 2;

    public DeleteRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    [Fact]
    public async void SuccessfullyAddNewTransaction()
    {
        _mockUserAuthService.Setup(x => x.DecodeToken(_tokenToDecode)).Returns(Task.FromResult(_authedUser));
        _mockRegisterDatabase.Setup(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId)).Returns(Task.FromResult(true));

        var newTransactionRequest = new DeleteTransactionRequest(_transactionId);

        await _registerService.DeleteTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserAuthService.Verify(x => x.DecodeToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.DeleteTransaction(_transactionId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
