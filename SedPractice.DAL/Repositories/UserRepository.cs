using SedPractice.DAL.Infrastructure;
using SedPractice.Domain.Enums;
using SedPractice.Domain.Models;

namespace SedPractice.DAL.Repositories;

public sealed class UserRepository
{
    private readonly SqliteConnectionFactory _factory;

    public UserRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public User? GetByLogin(string login)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Login, PasswordHash, FullName, Department, Role
            FROM Users
            WHERE Login = @login
        """;
        command.Parameters.AddWithValue("@login", login);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            Login = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            FullName = reader.GetString(3),
            Department = reader.GetString(4),
            Role = (UserRole)reader.GetInt32(5)
        };
    }
}
