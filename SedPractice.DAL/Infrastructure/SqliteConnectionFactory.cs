using Microsoft.Data.Sqlite;

namespace SedPractice.DAL.Infrastructure;

public sealed class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string databasePath)
    {
        _connectionString = $"Data Source={databasePath}";
    }

    public SqliteConnection Create()
    {
        return new SqliteConnection(_connectionString);
    }
}
