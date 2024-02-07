using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace CarterAndMVC.Database
{
    public partial class SQLiteDbService : IDisposable
    {
        private IDbConnection _connection;
        private readonly string _connectionString;

        public List<Dictionary<string, object>> GetData(string query)
        {
            OpenConnection();
            using (var command = new SQLiteCommand(query, (SQLiteConnection)_connection))
            {
                using (var adapter = new SQLiteDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Convert the DataTable to a list of dictionaries
                    var data = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in dataTable.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }
                        data.Add(dict);
                    }

                    return data;
                }
            }
        }
    }
}