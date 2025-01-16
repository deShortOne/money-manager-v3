
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Commands.Domain.Entities.Account;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Contracts.Requests.Transaction;
using Moq;

namespace MoneyTracker.Commands.Tests.RegisterTests.Service;
public sealed class EditRegisterTest : RegisterTestHelper
{
    private readonly int _userId = 52;
    private readonly AuthenticatedUser _authedUser;
    private readonly string _tokenToDecode = "tokenToDecode";

    private readonly int _transactionId = 2;

    public EditRegisterTest()
    {
        _authedUser = new AuthenticatedUser(_userId);
    }

    public static TheoryData<int?, int?, DateOnly?, int?, int?> OnlyOneItemNotNull = new() {
        { 745, null, null, null, null },
        { null, 500, null, null, null },
        { null, null, new DateOnly(2023, 2, 1), null, null },
        { null, null, null, 3, null },
        { null, null, null, null, 1 },
    };

    [Theory, MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditTransactionOneItemOnly(int? payee, int? amount, DateOnly? datePaid, int? categoryId, int? payerId)
    {
        var commonTransactionId = 714;

        _mockUserService.Setup(x => x.GetUserFromToken(_tokenToDecode))
            .ReturnsAsync(_authedUser);

        if (payerId != null)
            _mockAccountDatabase.Setup(x => x.GetAccountById((int)payerId)).ReturnsAsync(new AccountEntity(1, "", _userId));

        var editTransactionRequest = new EditTransactionRequest(_transactionId, payee, amount, datePaid, categoryId, payerId);
        var editTransaction = new EditTransactionEntity(_transactionId, payee, amount, datePaid, categoryId, payerId);

        _mockRegisterDatabase.Setup(x => x.GetTransaction(_transactionId))
            .ReturnsAsync(new TransactionEntity(commonTransactionId, -1, -1, new DateOnly(), -1, commonTransactionId));
        _accountService.Setup(x => x.DoesUserOwnAccount(_authedUser, commonTransactionId))
            .ReturnsAsync(true);

        await _registerService.EditTransaction(_tokenToDecode, editTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserService.Verify(x => x.GetUserFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.EditTransaction(editTransaction), Times.Once);
            _mockRegisterDatabase.Verify(x => x.GetTransaction(_transactionId), Times.Once);
            _accountService.Verify(x => x.DoesUserOwnAccount(_authedUser, commonTransactionId), Times.Once);

            if (payerId != null)
                _mockAccountDatabase.Verify(x => x.GetAccountById((int)payerId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
