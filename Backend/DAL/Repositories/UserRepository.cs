using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Extensions.Primitives;
using Npgsql;


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

    public async Task<User?> GetUserByIdAsync(int id)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter((NpgsqlConnection)connection,
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
            await using var connection = new NpgsqlConnection(_connectionString);
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
            await using var connection = new NpgsqlConnection(_connectionString);
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
    

    #endregion

    public async Task<User?> CreateUserAsync(User user)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            var query = new StringBuilder()
                .Append("INSERT INTO users (user_name, first_name, last_name, email, password,  is_verified) ")
                .Append("VALUES            (@userName, @firstName, @lastName, @email, @password,  @isVerified)");
            injector.InjectParameters(query, new Dictionary<string, object>
            {
                {"@userName", user.UserName},
                {"@firstName", user.FirstName},
                {"@lastName", user.LastName},
                {"@email", user.Email},
                {"@password", user.Password},
                {"@isVerified", user.IsVerified}
            });
            
            command.CommandText = query.ToString();
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? user : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding user", e);
        }
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            var query = new StringBuilder()
                .Append("UPDATE users SET user_name = @userName, first_name = @firstName, ")
                .Append("last_name = @lastName, email = @email, is_verified = @isVerified ");

            if (user.Password != null)
            {
                query.Append(" ,password = @password");
                injector.InjectParameters(query, new Dictionary<string, object>
                {
                    {"@password", user.Password}
                });
                
            }
            query.Append("WHERE user_id = @user_id");
            injector.InjectParameters(query, new Dictionary<string, object>
            {
                {"@userName", user.UserName},
                {"@firstName", user.FirstName},
                {"@lastName", user.LastName},
                {"@email", user.Email},
                {"@isVerified", user.IsVerified},
                {"@user_id", id}
            });
            
            command.CommandText = query.ToString();
            
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
            await using var connection = new NpgsqlConnection(_connectionString);
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