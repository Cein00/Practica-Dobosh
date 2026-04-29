using SedPractice.BLL.DTOs;
using SedPractice.DAL.Repositories;
using SedPractice.DAL.Security;

namespace SedPractice.BLL.Services;

public sealed class AuthService
{
    private readonly UserRepository _userRepository;

    public AuthService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public LoginResult Login(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            return LoginResult.Fail("Логин и пароль обязательны.");
        }

        var user = _userRepository.GetByLogin(login.Trim());
        if (user is null)
        {
            return LoginResult.Fail("Пользователь не найден.");
        }

        var passwordHash = PasswordHasher.ComputeHash(password);
        if (!string.Equals(user.PasswordHash, passwordHash, StringComparison.OrdinalIgnoreCase))
        {
            return LoginResult.Fail("Неверный пароль.");
        }

        return LoginResult.Success(user);
    }
}
