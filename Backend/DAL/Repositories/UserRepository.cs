using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;

namespace DAL.Repositories;

public class UserRepository(
    DatabaseSettings configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    #region GettingData

    public async Task<User?> GetUserByIdAsync(long id)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter(connection,
                "SELECT * FROM users WHERE user_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUser(dataTable.Rows[0]) : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting user by id", e);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter(connection,
                "SELECT * FROM users WHERE email = @email", "@email",
                email);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUser(dataTable.Rows[0]) : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting user by email", e);
        }
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter(connection,
                "SELECT * FROM users WHERE user_name = @userName",
                "@userName", userName);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUser(dataTable.Rows[0]) : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting user by username", e);
        }
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTable(connection, "SELECT * FROM users");
            return (from DataRow row in dataTable.Rows select entityCreator.CreateUser(row)).ToList();
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting all users", e);
        }
    }

    #endregion

    public async Task<User?> CreateUserAsync(User user)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO users (user_name, first_name, last_name, email, password, updated_at, created_at, last_login_at, reset_token_expiry, reset_token, is_verified)" +
                "VALUES (@userName, @firstName, @lastName, @email, @password, @updatedAt, @createdAt, @lastLoginAt, @resetTokenExpiry, @resetToken, @isVerified)";
            injector.FillUserEntityParameters(user, command);
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? user : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding user", e);
        }
    }

    public async Task<User?> UpdateUserAsync(long id, User user)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE users SET user_name = @userName, first_name = @firstName, last_name = @lastName, email = @email, password = @password, updated_at = @updatedAt, last_login_at = @lastLoginAt, reset_token_expiry = @resetTokenExpiry, reset_token = @resetToken, is_verified = @isVerified WHERE user_id = @id";
            injector.FillUserEntityParameters(user, command);
            command.Parameters.AddWithValue("@id", id);
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? user : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating user", e);
        }
    }

    public async Task<User?> DeleteUserAsync(long id)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE user_id = @id";
            command.Parameters.AddWithValue("@id", id);
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? new User() : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while deleting user", e);
        }
    }
}