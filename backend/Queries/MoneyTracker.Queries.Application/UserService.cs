
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;
using MoneyTracker.Queries.Domain.Handlers;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> GetUserToken(LoginWithUsernameAndPassword userLogin)
    {
        var user = await _userRepository.GetUserByUsername(userLogin.Username);
        if (user == null)
            throw new InvalidDataException("User does not exist");
        if (!_passwordHasher.VerifyPassword(user.Password, userLogin.Password, ""))
            throw new InvalidDataException("User does not exist");

        var token = await _userRepository.GetLastUserTokenForUser(user);
        if (token == null)
            throw new InvalidDataException("Token not found");
        return token;
    }

    public async Task<bool> IsTokenValid(string token)
    {
        var userAuth = await _userRepository.GetUserAuthFromToken(token);
        if (userAuth == null)
            return false;
        var userAuthResult = userAuth.CheckValidation();

        return userAuthResult.IsSuccess;
    }
}
