using DAL.Entities;
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
                                ?? throw new ArgumentNullException(nameof(configuration),
                                    "Connection string not found in configuration");
        }


        public async Task<UserEntity?> GetUserByIdAsync(int id)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM User WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new UserEntity
                    {
                        UserId = reader.GetInt32(0),
                        UserName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Email = reader.GetString(4),
                        Password = reader.GetString(5),
                        UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                        CreatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                        LastLoginAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        ResetTokenExpiry = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                        ResetToken = reader.IsDBNull(10) ? null : reader.GetString(10),
                        IsVerified = reader.GetBoolean(11)
                    };
                }

                return null;
            }
        }

        public async Task<UserEntity> GetUserWithDetails(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT u.*, p.*
                    FROM user AS u
                    INNER JOIN profiles AS p ON u.user_id = p.profile_id
                    INNER JOIN pictures AS p ON u.user_id = p.profile_id
                    WHERE u.user_id = @id";
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new UserEntity
                    {
                        UserId = reader.GetInt32(0),
                        UserName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Email = reader.GetString(4),
                        Password = reader.GetString(5),
                        UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                        CreatedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                        LastLoginAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        ResetTokenExpiry = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
                        ResetToken = reader.IsDBNull(10) ? null : reader.GetString(10),
                        IsVerified = reader.GetBoolean(11)

                        UserProfile = new UserProfileEntity
                        {
                        ProfileId = reader.GetInt32(12),
                    }
                    };
                }

                return null;
            }
        }

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