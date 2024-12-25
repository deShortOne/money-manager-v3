
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Entities.Transaction;
using MoneyTracker.Common.Utilities.DateTimeUtil;
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

    public static TheoryData<string, int?, DateOnly?, int?, int?> OnlyOneItemNotNull = new() {
        { "payee", null, null, null, null },
        { null, 500, null, null, null },
        { null, null, new DateOnly(2023, 2, 1), null, null },
        { null, null, null, 3, null },
        { null, null, null, null, 1 },
    };

    [Theory,  MemberData(nameof(OnlyOneItemNotNull))]
    public async void EditTransactionOneItemOnly(string payee, int? amount, DateOnly? datePaid, int? categoryId, int? accountId)
    {
        var mockDateTime = new Mock<IDateTimeProvider>();
        mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 6, 6, 10, 0, 0));
        _mockUserRepository.Setup(x => x.GetUserAuthFromToken(_tokenToDecode))
            .Returns(Task.FromResult(new UserAuthentication(new UserEntity(_userId, "", ""), _tokenToDecode, 
            new DateTime(2024, 6, 6, 10, 0, 0), mockDateTime.Object)));
            
        _mockRegisterDatabase.Setup(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId)).Returns(Task.FromResult(true));
        if (accountId != null)
            _mockAccountDatabase.Setup(x => x.IsAccountOwnedByUser(_authedUser, (int)accountId)).Returns(Task.FromResult(true));

        var editTransactionRequest = new EditTransactionRequest(_transactionId, payee, amount, datePaid, categoryId, accountId);
        var editTransaction = new EditTransactionEntity(_transactionId, payee, amount, datePaid, categoryId, accountId);

        await _registerService.EditTransaction(_tokenToDecode, editTransactionRequest);

        Assert.Multiple(() =>
        {
            _mockUserRepository.Verify(x => x.GetUserAuthFromToken(_tokenToDecode), Times.Once);
            _mockRegisterDatabase.Verify(x => x.IsTransactionOwnedByUser(_authedUser, _transactionId), Times.Once);
            _mockRegisterDatabase.Verify(x => x.EditTransaction(editTransaction), Times.Once);
            
            if (accountId != null)
                _mockAccountDatabase.Verify(x => x.IsAccountOwnedByUser(_authedUser, (int)accountId), Times.Once);

            EnsureAllMocksHadNoOtherCalls();
        });
    }
}
