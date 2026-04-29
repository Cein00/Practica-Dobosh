using SedPractice.DAL.Infrastructure;
using SedPractice.Domain.Models;

namespace SedPractice.DAL.Repositories;

public sealed class AuditLogRepository
{
    private readonly SqliteConnectionFactory _factory;

    public AuditLogRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Add(AuditLog auditLog)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO AuditLogs (CreatedAt, UserLogin, Action, EntityName, EntityKey, Description)
            VALUES (@createdAt, @userLogin, @action, @entityName, @entityKey, @description)
        """;
        command.Parameters.AddWithValue("@createdAt", auditLog.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@userLogin", auditLog.UserLogin);
        command.Parameters.AddWithValue("@action", auditLog.Action);
        command.Parameters.AddWithValue("@entityName", auditLog.EntityName);
        command.Parameters.AddWithValue("@entityKey", auditLog.EntityKey);
        command.Parameters.AddWithValue("@description", auditLog.Description);
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<AuditLog> GetRecent(int limit = 50)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, CreatedAt, UserLogin, Action, EntityName, EntityKey, Description
            FROM AuditLogs
            ORDER BY Id DESC
            LIMIT @limit
        """;
        command.Parameters.AddWithValue("@limit", limit);

        using var reader = command.ExecuteReader();
        var result = new List<AuditLog>();
        while (reader.Read())
        {
            result.Add(new AuditLog
            {
                Id = reader.GetInt32(0),
                CreatedAt = DateTime.Parse(reader.GetString(1)),
                UserLogin = reader.GetString(2),
                Action = reader.GetString(3),
                EntityName = reader.GetString(4),
                EntityKey = reader.GetString(5),
                Description = reader.GetString(6)
            });
        }

        return result;
    }
}
