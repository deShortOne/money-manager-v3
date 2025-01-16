
using Microsoft.AspNetCore.Http.HttpResults;
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Authentication.Entities;
using MoneyTracker.Authentication.Interfaces;
using MoneyTracker.Commands.Domain.Handlers;
using MoneyTracker.Commands.Domain.Repositories;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Common.Result;
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

    public async Task<Result> AddNewUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        var lastUserId = await _userRepository.GetLastUserId();
        var newUserId = _idGenerator.NewInt(lastUserId);
        await _userRepository.AddUser(new UserEntity(newUserId, usernameAndPassword.Username, usernameAndPassword.Password));

        return Result.Success();
    }

    public async Task<Result> LoginUser(LoginWithUsernameAndPassword usernameAndPassword)
    {
        var user = await _userRepository.GetUserByUsername(usernameAndPassword.Username);
        if (user == null)
            return Result.Failure(Error.NotFound("UserService.LoginUser", "User does not exist"));
        if (!_passwordHasher.VerifyPassword(user.Password, usernameAndPassword.Password, ""))
            return Result.Failure(Error.NotFound("UserService.LoginUser", "User does not exist"));

        var expiration = _dateTimeProvider.Now.AddMinutes(ExpirationTimeInMinutesForAll);
        var tokenToReturn = _authenticationService.GenerateToken(new UserIdentity(user.Id.ToString()), expiration);
        await _userRepository.StoreTemporaryTokenToUser(new UserAuthentication(user, tokenToReturn, expiration, _dateTimeProvider));

        return Result.Success();
    }

    public async Task<ResultT<AuthenticatedUser>> GetUserFromToken(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            return ResultT<AuthenticatedUser>
                .Failure(Error.AccessUnAuthorised("UserService.GetUserFromToken", "Token not found"));

        var userValidationResult = userAuth.CheckValidation();
        if (!userValidationResult.IsSuccess)
            return ResultT<AuthenticatedUser>
                .Failure(userValidationResult.Error!);

        return ResultT<AuthenticatedUser>.Success(new AuthenticatedUser(userAuth.User.Id));
    }
}
