using SedPractice.BLL.Services;
using SedPractice.ConsoleApp;

namespace SedPractice.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public void Login_WithValidCredentials_ReturnsSuccess()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"sed-test-{Guid.NewGuid():N}.db");
        var host = new AppHost(databasePath);

        var result = host.AuthService.Login("admin", "admin123");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.User);
        Assert.Equal("admin", result.User!.Login);
    }

    [Fact]
    public void Login_WithInvalidPassword_ReturnsFail()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"sed-test-{Guid.NewGuid():N}.db");
        var host = new AppHost(databasePath);

        var result = host.AuthService.Login("admin", "wrong-password");

        Assert.False(result.IsSuccess);
    }
}
