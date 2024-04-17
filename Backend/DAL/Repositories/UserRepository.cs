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
                command.CommandText = "SELECT * FROM users WHERE user_id = @id";
                command.Parameters.AddWithValue("@id", id);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return CreateUserEntity(reader);
                }

                return null;
            }
        }
        
        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users";
                var reader = await command.ExecuteReaderAsync();
                var users = new List<UserEntity>();
                while (await reader.ReadAsync())
                {
                    users.Add(CreateUserEntity(reader));
                }

                return users;
            }
        }
        
        public async Task<UserEntity?> GetUserByUserNameAsync(string userName)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE user_name = @userName";
                command.Parameters.AddWithValue("@userName", userName);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return CreateUserEntity(reader);
                }

                return null;
            }
        }
        
        public async Task<UserEntity?> AddUserAsync(UserEntity user)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO users (user_name, first_name, last_name, email, password, updated_at, created_at, last_login_at, reset_token_expiry, reset_token, is_verified)" +
                                      "VALUES (@userName, @firstName, @lastName, @email, @password, @updatedAt, @createdAt, @lastLoginAt, @resetTokenExpiry, @resetToken, @isVerified)";
                FillUserEntityParameters(user, command);
                await command.ExecuteNonQueryAsync();
                return user;
            }
        }
        
        public async Task<UserEntity?> UpdateUserAsync(int id, UserEntity user)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET user_name = @userName, first_name = @firstName, last_name = @lastName, email = @email, password = @password, " +
                                      "updated_at = @updatedAt, created_at = @createdAt, last_login_at = @lastLoginAt, reset_token_expiry = @resetTokenExpiry, reset_token = @resetToken, is_verified = @isVerified "+
                                      "WHERE Id = @id";
                FillUserEntityParameters(user, command);
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
                return user;
            }
        }
        
        public async Task DeleteUserAsync(int id)
        {
            await using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM User WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static void FillUserEntityParameters(UserEntity user, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@userName", user.UserName);
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@updatedAt", user.UpdatedAt ?? DateTime.Now);
            command.Parameters.AddWithValue("@createdAt", user.CreatedAt ?? DateTime.Now);
            command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt ?? DateTime.Now);
            command.Parameters.AddWithValue("@resetTokenExpiry", user.ResetTokenExpiry ?? DateTime.Now);
            command.Parameters.AddWithValue("@resetToken", user.ResetToken ?? "");
            command.Parameters.AddWithValue("@isVerified", user.IsVerified ?? false);
        }


        private static UserEntity CreateUserEntity(SqliteDataReader reader)
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


        
    }
}