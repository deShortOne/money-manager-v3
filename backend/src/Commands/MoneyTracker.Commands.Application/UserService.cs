using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
using MoneyTracker.Common.Utilities.DateTimeUtil;
using MoneyTracker.Common.Utilities.IdGeneratorUtil;
using MoneyTracker.PlatformService.Domain;
using MoneyTracker.PlatformService.DTOs;

namespace MoneyTracker.Commands.Application;

public class UserService : IUserService
{
    private readonly IUserCommandRepository _userRepository;
    private readonly IIdGenerator _idGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMessageBusClient _messageBus;
    private const int ExpirationTimeInMinutesForAll = 60;

    public UserService(IUserCommandRepository userRepository,
        IIdGenerator idGenerator,
        IPasswordHasher passwordHasher,
        IAuthenticationService authenticationService,
        IDateTimeProvider dateTimeProvider,
        IMessageBusClient messageBus
        )
    {
        _userRepository = userRepository;
        _idGenerator = idGenerator;
        _passwordHasher = passwordHasher;
        _authenticationService = authenticationService;
        _dateTimeProvider = dateTimeProvider;
        _messageBus = messageBus;
    }

    public async Task<Result> AddNewUser(LoginWithUsernameAndPassword usernameAndPassword,
        CancellationToken cancellationToken)
    {
        var lastUserId = await _userRepository.GetLastUserId(cancellationToken);
        var newUserId = _idGenerator.NewInt(lastUserId);
        var hashedPassword = _passwordHasher.HashPassword(usernameAndPassword.Password);
        await _userRepository.AddUser(new UserEntity(newUserId, usernameAndPassword.Username, hashedPassword), cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(newUserId), DataTypes.User), cancellationToken);

        return Result.Success();
    }

    public async Task<Result> LoginUser(LoginWithUsernameAndPassword usernameAndPassword,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsername(usernameAndPassword.Username, cancellationToken);
        if (user == null)
            return Error.NotFound("UserService.LoginUser", "User does not exist");
        if (!_passwordHasher.VerifyPassword(user.Password, usernameAndPassword.Password))
            return Error.NotFound("UserService.LoginUser", "User does not exist");

        var expiration = _dateTimeProvider.Now.AddMinutes(ExpirationTimeInMinutesForAll);
        var tokenToReturn = _authenticationService.GenerateToken(new UserIdentity(user.Id.ToString()), expiration);
        await _userRepository.StoreTemporaryTokenToUser(new UserAuthentication(user, tokenToReturn, expiration, _dateTimeProvider), cancellationToken);

        await _messageBus.PublishEvent(new EventUpdate(new AuthenticatedUser(user.Id), DataTypes.User), cancellationToken);

        return Result.Success();
    }

    public async Task<ResultT<AuthenticatedUser>> GetUserFromToken(string token, CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            return Error.AccessUnAuthorised("UserService.GetUserFromToken", "Token not found");

        var userValidationResult = userAuth.CheckValidation();
        if (userValidationResult.HasError)
            return userValidationResult.Error!;

        return new AuthenticatedUser(userAuth.User.Id);
    }
}
