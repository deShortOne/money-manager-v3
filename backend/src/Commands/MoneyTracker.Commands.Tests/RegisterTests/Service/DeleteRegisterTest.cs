
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
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
    public async Task SuccessfullyAddNewTransaction()
    {
        var commonAccountId = 1562;

        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None))
            .ReturnsAsync(_authedUser);

        var newTransactionRequest = new DeleteTransactionRequest(_transactionId);

        _mockRegisterDatabase.Setup(x => x.GetTransaction(_transactionId, CancellationToken.None))
            .ReturnsAsync(new TransactionEntity(_transactionId, -1, -1, new DateOnly(), -1, commonAccountId));

        _accountService.Setup(x => x.DoesUserOwnAccount(_authedUser, commonAccountId, CancellationToken.None))
            .ReturnsAsync(true);

        await _registerService.DeleteTransaction(_tokenToDecode, newTransactionRequest, CancellationToken.None);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode, CancellationToken.None), Times.Once);
            _mockRegisterDatabase.Verify(x => x.DeleteTransaction(_transactionId, CancellationToken.None), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetTransaction(_transactionId, CancellationToken.None), Times.Once);
            _accountService.Verify(x => x.DoesUserOwnAccount(_authedUser, commonAccountId, CancellationToken.None), Times.Once);

            _mockMessageBusClient.Verify(x => x.PublishEvent(
                new EventUpdate(_authedUser, DataTypes.Register), It.IsAny<CancellationToken>()
                ), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
