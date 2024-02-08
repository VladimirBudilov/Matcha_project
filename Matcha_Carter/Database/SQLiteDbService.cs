using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;


namespace CarterAndMVC.Database
{
    public partial class SQLiteDbService : IDisposable
    {
        private IDbConnection _connection;
        private readonly string _connectionString;

        public List<Dictionary<string, object>> GetData(string query)
        {
            OpenConnection();
            using (var command = new SqliteCommand(query, (SqliteConnection)_connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var result = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(row);
                    }
                    return result;
                }                
            }
        }
    }
}