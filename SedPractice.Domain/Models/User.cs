using SedPractice.Domain.Enums;

namespace SedPractice.Domain.Models;

public sealed class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
