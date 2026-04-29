using SedPractice.DAL.Infrastructure;
using SedPractice.Domain.Models;

namespace SedPractice.DAL.Repositories;

public sealed class ApprovalTaskRepository
{
    private readonly SqliteConnectionFactory _factory;

    public ApprovalTaskRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public int Create(ApprovalTask task)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO ApprovalTasks
            (DocumentId, ApproverLogin, CreatedAt, Deadline, IsCompleted, Resolution, Comment)
            VALUES
            (@documentId, @approverLogin, @createdAt, @deadline, @isCompleted, @resolution, @comment);
            SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("@documentId", task.DocumentId);
        command.Parameters.AddWithValue("@approverLogin", task.ApproverLogin);
        command.Parameters.AddWithValue("@createdAt", task.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@deadline", task.Deadline.ToString("O"));
        command.Parameters.AddWithValue("@isCompleted", task.IsCompleted ? 1 : 0);
        command.Parameters.AddWithValue("@resolution", (object?)task.Resolution ?? DBNull.Value);
        command.Parameters.AddWithValue("@comment", (object?)task.Comment ?? DBNull.Value);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public ApprovalTask? GetOpenTaskByDocumentId(int documentId)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, DocumentId, ApproverLogin, CreatedAt, Deadline, IsCompleted, Resolution, Comment
            FROM ApprovalTasks
            WHERE DocumentId = @documentId
              AND IsCompleted = 0
            ORDER BY Id DESC
            LIMIT 1
        """;
        command.Parameters.AddWithValue("@documentId", documentId);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public IReadOnlyList<ApprovalTask> GetOverdueTasks()
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, DocumentId, ApproverLogin, CreatedAt, Deadline, IsCompleted, Resolution, Comment
            FROM ApprovalTasks
            WHERE IsCompleted = 0
              AND Deadline < @now
            ORDER BY Deadline ASC
        """;
        command.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("O"));

        using var reader = command.ExecuteReader();
        var result = new List<ApprovalTask>();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public void Complete(int taskId, string resolution, string? comment)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE ApprovalTasks
            SET IsCompleted = 1,
                Resolution = @resolution,
                Comment = @comment
            WHERE Id = @id
        """;
        command.Parameters.AddWithValue("@resolution", resolution);
        command.Parameters.AddWithValue("@comment", (object?)comment ?? DBNull.Value);
        command.Parameters.AddWithValue("@id", taskId);
        command.ExecuteNonQuery();
    }

    private static ApprovalTask Map(Microsoft.Data.Sqlite.SqliteDataReader reader)
    {
        return new ApprovalTask
        {
            Id = reader.GetInt32(0),
            DocumentId = reader.GetInt32(1),
            ApproverLogin = reader.GetString(2),
            CreatedAt = DateTime.Parse(reader.GetString(3)),
            Deadline = DateTime.Parse(reader.GetString(4)),
            IsCompleted = reader.GetInt32(5) == 1,
            Resolution = reader.IsDBNull(6) ? null : reader.GetString(6),
            Comment = reader.IsDBNull(7) ? null : reader.GetString(7)
        };
    }
}
