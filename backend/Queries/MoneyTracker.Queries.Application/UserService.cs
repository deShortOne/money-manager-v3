
using MoneyTracker.Authentication.DTOs;
using MoneyTracker.Common.Interfaces;

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

        return await _userRepository.GetUserToken(user);
    }
}