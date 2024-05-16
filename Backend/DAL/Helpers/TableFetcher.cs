﻿using System.Data;
using Npgsql;

namespace DAL.Helpers;

public class TableFetcher
{
    public async Task<DataTable> GetTableByParameter(NpgsqlConnection connection, string sqlQuery,
        string parameterName,
        long parameter)
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

    public async Task<DataTable> GetTable(NpgsqlConnection connection, string selectFromUsers)
    {
        var command = connection.CreateCommand();
        command.CommandText = selectFromUsers;
        var readerAsync = await command.ExecuteReaderAsync();
        var table = new DataTable();
        if (readerAsync.HasRows)
        {
            table.Load(readerAsync);
        }
        return table;
    }
}