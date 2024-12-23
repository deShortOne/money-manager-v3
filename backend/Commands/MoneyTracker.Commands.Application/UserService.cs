
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;

namespace MoneyTracker.Commands.Application;

public class UserService : IUserService
{
    private readonly IUserCommandRepository _userRepository;
    private readonly IIdGenerator _idGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private const int ExpirationTimeInMinutesForAll = 60;

    public UserService(IUserCommandRepository userRepository,
        IIdGenerator idGenerator,
        IPasswordHasher passwordHasher,
        IAuthenticationService authenticationService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _idGenerator = idGenerator;
        _passwordHasher = passwordHasher;
        _authenticationService = authenticationService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task AddNewUser(LoginWithUsernameAndPassword usernameAndPassword) 
    {
        var lastUserId = await _userRepository.GetLastUserId();
        var newUserId = _idGenerator.NewInt(lastUserId);
        await _userRepository.AddUser(new UserEntity(newUserId, usernameAndPassword.Username, usernameAndPassword.Password));
    }

    public async Task LoginUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        var user = await _userRepository.GetUserByUsername(usernameAndPassword.Username);
        if (user == null)
            throw new InvalidDataException("User does not exist");
        if (!_passwordHasher.VerifyPassword(user.Password, usernameAndPassword.Password, ""))
            throw new InvalidDataException("User does not exist");
        
        var expiration = _dateTimeProvider.Now.AddMinutes(ExpirationTimeInMinutesForAll);
        var tokenToReturn = _authenticationService.GenerateToken(new UserIdentity(user.Id.ToString()), expiration);
        
        await _userRepository.StoreTemporaryTokenToUser(new UserAuthentication(user, tokenToReturn, expiration));
    }
}
