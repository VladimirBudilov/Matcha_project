using System.Data;
using Microsoft.Data.Sqlite;

namespace CarterAndMVC.Database;

public partial class SQLiteDbService
{
    public SQLiteDbService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void OpenConnection()
    {
        if (_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
    }

    public IDbConnection GetConnection()
    {
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}