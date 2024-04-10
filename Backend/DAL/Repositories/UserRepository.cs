using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UserDbConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found in configuration");
        }

        //create connection to database and get all users

        public IEnumerable<string> GetAllUsers()
        {
           using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM User";
                var reader = command.ExecuteReader();
                var users = new List<string>();
                while (reader.Read())
                {
                    var id = reader["Id"];
                    var name = reader["Name"];
                    var surname = reader["Surname"];

                    users.Add($"{id} {name} {surname}");
                }

                return users;
           }

        }
    }
}