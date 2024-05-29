using System.Data;
using Npgsql;

namespace DAL.Helpers;

public class TableFetcher(DatabaseSettings settings)
{
    private readonly string connectionString = settings.ConnectionString;
    
    public async Task<DataTable> GetTableByParameter(string sqlQuery,
        string parameterName,
        int parameter)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var command =connection.CreateCommand();
        command.CommandText = sqlQuery;
        command.Parameters.AddWithValue(parameterName, parameter);
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }
        
    public  async Task<DataTable> GetTableByParameter( string sqlQuery,
        string parameterName,
        string parameter)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var command =connection.CreateCommand();
        command.CommandText = sqlQuery;
        command.Parameters.AddWithValue(parameterName, parameter);
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }

    public async Task<DataTable> GetTableAsync( string query)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var command =connection.CreateCommand();
        command.CommandText = query;
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }

    public async Task<DataTable> GetTableByParameter( string query, Dictionary<string, object> parameters)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var command =connection.CreateCommand();
        command.CommandText = query;
        foreach (var (key, value) in parameters)
        {
            command.Parameters.AddWithValue(key, value);
        }
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }
    
}