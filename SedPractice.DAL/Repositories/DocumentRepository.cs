using SedPractice.DAL.Infrastructure;
using SedPractice.Domain.Enums;
using SedPractice.Domain.Models;

namespace SedPractice.DAL.Repositories;

public sealed class DocumentRepository
{
    private readonly SqliteConnectionFactory _factory;

    public DocumentRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public int Create(Document document)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Documents
            (RegistrationNumber, Title, DocumentType, AuthorLogin, CreatedAt, DueDate, Status, CurrentVersion, Content)
            VALUES
            (@number, @title, @type, @author, @createdAt, @dueDate, @status, @version, @content);
            SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("@number", document.RegistrationNumber);
        command.Parameters.AddWithValue("@title", document.Title);
        command.Parameters.AddWithValue("@type", document.DocumentType);
        command.Parameters.AddWithValue("@author", document.AuthorLogin);
        command.Parameters.AddWithValue("@createdAt", document.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("@dueDate", document.DueDate?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@status", (int)document.Status);
        command.Parameters.AddWithValue("@version", document.CurrentVersion);
        command.Parameters.AddWithValue("@content", document.Content);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public IReadOnlyList<Document> GetAll()
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, RegistrationNumber, Title, DocumentType, AuthorLogin, CreatedAt, DueDate, Status, CurrentVersion, Content
            FROM Documents
            ORDER BY CreatedAt DESC
        """;

        using var reader = command.ExecuteReader();
        var result = new List<Document>();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public IReadOnlyList<Document> Search(string searchTerm)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, RegistrationNumber, Title, DocumentType, AuthorLogin, CreatedAt, DueDate, Status, CurrentVersion, Content
            FROM Documents
            WHERE RegistrationNumber LIKE @term
               OR Title LIKE @term
               OR AuthorLogin LIKE @term
               OR DocumentType LIKE @term
            ORDER BY CreatedAt DESC
        """;
        command.Parameters.AddWithValue("@term", $"%{searchTerm}%");

        using var reader = command.ExecuteReader();
        var result = new List<Document>();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public Document? GetById(int id)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, RegistrationNumber, Title, DocumentType, AuthorLogin, CreatedAt, DueDate, Status, CurrentVersion, Content
            FROM Documents
            WHERE Id = @id
        """;
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public void UpdateStatus(int documentId, DocumentStatus status, string version)
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Documents
            SET Status = @status,
                CurrentVersion = @version
            WHERE Id = @id
        """;
        command.Parameters.AddWithValue("@status", (int)status);
        command.Parameters.AddWithValue("@version", version);
        command.Parameters.AddWithValue("@id", documentId);
        command.ExecuteNonQuery();
    }

    private static Document Map(Microsoft.Data.Sqlite.SqliteDataReader reader)
    {
        return new Document
        {
            Id = reader.GetInt32(0),
            RegistrationNumber = reader.GetString(1),
            Title = reader.GetString(2),
            DocumentType = reader.GetString(3),
            AuthorLogin = reader.GetString(4),
            CreatedAt = DateTime.Parse(reader.GetString(5)),
            DueDate = reader.IsDBNull(6) ? null : DateTime.Parse(reader.GetString(6)),
            Status = (DocumentStatus)reader.GetInt32(7),
            CurrentVersion = reader.GetString(8),
            Content = reader.GetString(9)
        };
    }
}
