using System.Security.Cryptography;
using System.Text;

namespace SedPractice.DAL.Security;

public static class PasswordHasher
{
    public static string ComputeHash(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
