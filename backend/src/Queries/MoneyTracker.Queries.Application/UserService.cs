
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Handlers;
using MoneyTracker.Queries.Domain.Repositories.Service;

namespace MoneyTracker.Queries.Application;
public class UserService : IUserService
{
    private readonly IUserRepositoryService _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepositoryService userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> GetUserToken(LoginWithUsernameAndPassword userLogin, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsername(userLogin.Username, cancellationToken);
        if (user == null)
            throw new InvalidDataException("User does not exist");
        if (!_passwordHasher.VerifyPassword(user.Password, userLogin.Password))
            throw new InvalidDataException("User does not exist");

        var token = await _userRepository.GetLastUserTokenForUser(user, cancellationToken);
        if (token == null)
            throw new InvalidDataException("Token not found");
        return token;
    }

    public async Task<bool> IsTokenValid(string token, CancellationToken cancellationToken)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token, cancellationToken);
        if (userAuth == null)
            return false;
        var userAuthResult = userAuth.CheckValidation();

        return userAuthResult.IsSuccess;
    }
}
