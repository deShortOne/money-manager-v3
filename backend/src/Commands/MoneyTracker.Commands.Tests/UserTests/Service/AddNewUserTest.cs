using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;
using Moq;

namespace MoneyTracker.Commands.Tests.UserTests.Service;
public sealed class AddNewUserTest : UserTestHelper
{
    private int _lastUserId = 2;
    private int _newUserId = 3;
    private string _username = "jkl";
    private string _password = "eior";
    private string _hashedPassword = "gdfassdf";

    [Fact]
    public async Task SuccessfullyAddNewUser()
    {
        _mockUserDatabase.Setup(x => x.GetLastUserId()).ReturnsAsync(_lastUserId);
        _mockIdGenerator.Setup(x => x.NewInt(_lastUserId)).Returns(_newUserId);
        _mockPasswordHasher.Setup(x => x.HashPassword(_password)).Returns(_hashedPassword);
        _mockUserDatabase.Setup(x => x.AddUser(new UserEntity(_newUserId, _username, _hashedPassword)));

        var result = await _userService.AddNewUser(new LoginWithUsernameAndPassword(_username, _password));

        Assert.True(result.IsSuccess);
        _mockUserDatabase.Verify(x => x.GetLastUserId(), Times.Once);
        _mockIdGenerator.Verify(x => x.NewInt(_lastUserId), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(_password), Times.Once);
        _mockUserDatabase.Verify(x => x.AddUser(new UserEntity(_newUserId, _username, _hashedPassword)), Times.Once);

        _mockMessageBusClient.Verify(x => x.PublishEvent(
            new EventUpdate(new AuthenticatedUser(_newUserId), DataTypes.User), It.IsAny<CancellationToken>()
            ), Times.Once);

        EnsureAllMocksHadNoOtherCalls();
    }
}
