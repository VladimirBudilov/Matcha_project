using System.Data;
using System.Data.SQLite;

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
            _connection = new SQLiteConnection(_connectionString);
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