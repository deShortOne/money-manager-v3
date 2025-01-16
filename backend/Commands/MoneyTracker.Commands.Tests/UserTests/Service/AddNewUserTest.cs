
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public sealed class AddNewUserTest : UserTestHelper
{
    private int _lastUserId = 2;
    private int _newUserId = 3;
    private string _username = "jkl";
    private string _password = "eior";

    [Fact]
    public async Task SuccessfullyAddNewUser()
    {
        _mockUserDatabase.Setup(x => x.GetLastUserId()).ReturnsAsync(_lastUserId);
        _mockIdGenerator.Setup(x => x.NewInt(_lastUserId)).Returns(_newUserId);
        _mockUserDatabase.Setup(x => x.AddUser(new UserEntity(_newUserId, _username, _password)));

        var result = await _userService.AddNewUser(new LoginWithUsernameAndPassword(_username, _password));

        Assert.True(result.IsSuccess);
        _mockUserDatabase.Verify(x => x.GetLastUserId(), Times.Once);
        _mockUserDatabase.Verify(x => x.AddUser(new UserEntity(_newUserId, _username, _password)), Times.Once);
        _mockIdGenerator.Verify(x => x.NewInt(_lastUserId), Times.Once);
        EnsureAllMocksHadNoOtherCalls();
    }
}
