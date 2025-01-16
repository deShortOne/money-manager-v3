
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
        var commonAccountId = 1562;

        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        var newTransactionRequest = new DeleteTransactionRequest(_transactionId);

        _mockRegisterDatabase.Setup(x => x.GetTransaction(_transactionId))
            .ReturnsAsync(new TransactionEntity(_transactionId, -1, -1, new DateOnly(), -1, commonAccountId));

        _accountService.Setup(x => x.DoesUserOwnAccount(_authedUser, commonAccountId))
            .ReturnsAsync(true);

        await _registerService.DeleteTransaction(_tokenToDecode, newTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.DeleteTransaction(_transactionId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetTransaction(_transactionId), Times.Once);
            _accountService.Verify(x => x.DoesUserOwnAccount(_authedUser, commonAccountId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
