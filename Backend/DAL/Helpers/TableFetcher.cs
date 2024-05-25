using System.Data;
using Npgsql;

namespace DAL.Helpers;

public class TableFetcher
{
    public async Task<DataTable> GetTableByParameter(NpgsqlConnection connection, string sqlQuery,
        string parameterName,
        int parameter)
    {
        var command = connection.CreateCommand();
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
        
    public  async Task<DataTable> GetTableByParameter(NpgsqlConnection connection, string sqlQuery,
        string parameterName,
        string parameter)
    {
        var command = connection.CreateCommand();
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

    public async Task<DataTable> GetTableAsync(NpgsqlConnection connection, string query)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }

    public async Task<DataTable> GetTableByParameter(NpgsqlConnection connection, string query, Dictionary<string, object> parameters)
    {
        var command = connection.CreateCommand();
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