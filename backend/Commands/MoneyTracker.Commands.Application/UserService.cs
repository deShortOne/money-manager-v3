
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Commands.Application;

public class UserService : IUserService
{
    private readonly IUserCommandRepository _userRepository;
    private readonly IIdGenerator _idGenerator;

    public UserService(IUserCommandRepository userRepository, IIdGenerator idGenerator)
    {
        _userRepository = userRepository;
        _idGenerator = idGenerator;
    }

    public async Task AddNewUser(LoginWithUsernameAndPassword usernameAndPassword) 
    {
        var lastUserId = await _userRepository.GetLastUserId();
        var newUserId = _idGenerator.NewInt(lastUserId);
        await _userRepository.AddUser(new UserEntity(newUserId, usernameAndPassword.Username, usernameAndPassword.Password));
    }
}
