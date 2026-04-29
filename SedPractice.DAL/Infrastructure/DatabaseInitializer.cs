using Microsoft.Data.Sqlite;
using SedPractice.Domain.Enums;
using SedPractice.DAL.Security;

namespace SedPractice.DAL.Infrastructure;

public sealed class DatabaseInitializer
{
    private readonly SqliteConnectionFactory _factory;

    public DatabaseInitializer(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Initialize()
    {
        using var connection = _factory.Create();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Login TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                FullName TEXT NOT NULL,
                Department TEXT NOT NULL,
                Role INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Documents (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                RegistrationNumber TEXT NOT NULL UNIQUE,
                Title TEXT NOT NULL,
                DocumentType TEXT NOT NULL,
                AuthorLogin TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                DueDate TEXT NULL,
                Status INTEGER NOT NULL,
                CurrentVersion TEXT NOT NULL,
                Content TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS ApprovalTasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DocumentId INTEGER NOT NULL,
                ApproverLogin TEXT NOT NULL,
                CreatedAt TEXT NOT NULL,
                Deadline TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL,
                Resolution TEXT NULL,
                Comment TEXT NULL,
                FOREIGN KEY (DocumentId) REFERENCES Documents(Id)
            );

            CREATE TABLE IF NOT EXISTS AuditLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CreatedAt TEXT NOT NULL,
                UserLogin TEXT NOT NULL,
                Action TEXT NOT NULL,
                EntityName TEXT NOT NULL,
                EntityKey TEXT NOT NULL,
                Description TEXT NOT NULL
            );
        """;
        command.ExecuteNonQuery();

        SeedUsers(connection);
        SeedDocuments(connection);
    }

    private static void SeedUsers(SqliteConnection connection)
    {
        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM Users";
        var count = Convert.ToInt32(checkCommand.ExecuteScalar());
        if (count > 0)
        {
            return;
        }

        InsertUser(connection, "admin", "admin123", "Сидоров Алексей Петрович", "Администрация", UserRole.Administrator);
        InsertUser(connection, "ivanov", "12345", "Иванов Илья Сергеевич", "Канцелярия", UserRole.Employee);
        InsertUser(connection, "petrov", "12345", "Петров Михаил Андреевич", "Руководство", UserRole.Manager);
    }

    private static void SeedDocuments(SqliteConnection connection)
    {
        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM Documents";
        var count = Convert.ToInt32(checkCommand.ExecuteScalar());
        if (count > 0)
        {
            return;
        }

        using var insert = connection.CreateCommand();
        insert.CommandText = """
            INSERT INTO Documents
            (RegistrationNumber, Title, DocumentType, AuthorLogin, CreatedAt, DueDate, Status, CurrentVersion, Content)
            VALUES
            ('ORD-2026-001', 'Приказ о назначении ответственного за архив', 'Приказ', 'ivanov', @createdAt, @dueDate, @status, '1.0', 'Назначить ответственного за ведение архива документов.'),
            ('MEMO-2026-004', 'Служебная записка о закупке МФУ', 'Служебная записка', 'ivanov', @createdAt2, @dueDate2, @status2, '1.0', 'Прошу согласовать закупку многофункционального устройства.');
        """;
        insert.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.AddDays(-5).ToString("O"));
        insert.Parameters.AddWithValue("@dueDate", DateTime.UtcNow.AddDays(2).ToString("O"));
        insert.Parameters.AddWithValue("@status", (int)DocumentStatus.Submitted);
        insert.Parameters.AddWithValue("@createdAt2", DateTime.UtcNow.AddDays(-2).ToString("O"));
        insert.Parameters.AddWithValue("@dueDate2", DateTime.UtcNow.AddDays(3).ToString("O"));
        insert.Parameters.AddWithValue("@status2", (int)DocumentStatus.Draft);
        insert.ExecuteNonQuery();
    }

    private static void InsertUser(SqliteConnection connection, string login, string password, string fullName, string department, UserRole role)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users (Login, PasswordHash, FullName, Department, Role)
            VALUES (@login, @passwordHash, @fullName, @department, @role)
        """;
        command.Parameters.AddWithValue("@login", login);
        command.Parameters.AddWithValue("@passwordHash", PasswordHasher.ComputeHash(password));
        command.Parameters.AddWithValue("@fullName", fullName);
        command.Parameters.AddWithValue("@department", department);
        command.Parameters.AddWithValue("@role", (int)role);
        command.ExecuteNonQuery();
    }
}
