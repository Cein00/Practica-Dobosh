using SedPractice.Domain.Models;

namespace SedPractice.BLL.DTOs;

public sealed class LoginResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public User? User { get; init; }

    public static LoginResult Success(User user) => new() { IsSuccess = true, User = user, Message = "Авторизация выполнена." };

    public static LoginResult Fail(string message) => new() { IsSuccess = false, Message = message };
}
