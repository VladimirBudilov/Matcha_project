using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Extensions.Primitives;
using Npgsql;


namespace DAL.Repositories;

public class UsersRepository(

    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    
    #region GettingData

    public async Task<User?> GetUserByIdAsync(int id)
    {
        try
        {
            var dataTable = await fetcher.GetTableByParameter("SELECT * FROM users WHERE user_id = @id", "@id", id);
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
            var dataTable = await fetcher.GetTableByParameter(
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
            var dataTable = await fetcher.GetTableByParameter(
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
            var query = new StringBuilder()
                .Append("INSERT INTO users (user_name, first_name, last_name, email, password,  is_verified) ")
                .Append("VALUES            (@userName, @firstName, @lastName, @email, @password,  @isVerified) Returning user_id");
            var parameters = new Dictionary<string, object>
            {
                {"@userName", user.UserName},
                {"@firstName", user.FirstName},
                {"@lastName", user.LastName},
                {"@email", user.Email},
                {"@password", user.Password},
                {"@isVerified", user.IsVerified}
            };
            
            var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
            var res = (int) table.Rows[0]["user_id"];
            return res > 0 ? user : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding user", e);
        }
    }
    
    public async Task<User?> CreateUserWithIdAsync(User user)
    {
        try
        {
            var query = new StringBuilder()
                .Append("INSERT INTO users (user_id , user_name, first_name, last_name, email, password,  is_verified) ")
                .Append("VALUES            (@user_id, @userName, @firstName, @lastName, @email, @password,  @isVerified) Returning user_id");
            var parameters = new Dictionary<string, object>
            {
                {"@user_id", user.Id},
                {"@userName", user.UserName},
                {"@firstName", user.FirstName},
                {"@lastName", user.LastName},
                {"@email", user.Email},
                {"@password", user.Password},
                {"@isVerified", user.IsVerified}
            };
            
            var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
            var res = (int) table.Rows[0]["user_id"];
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
            var query = new StringBuilder()
                .Append("UPDATE users SET user_name = @userName, first_name = @firstName, ")
                .Append("last_name = @lastName, email = @email, is_verified = @isVerified Returning user_id");

            var parameters = new Dictionary<string, object>
            {
                {"@userName", user.UserName},
                {"@firstName", user.FirstName},
                {"@lastName", user.LastName},
                {"@email", user.Email},
                {"@isVerified", user.IsVerified},
                {"@user_id", id}
            };
            
            if (user.Password != null)
            {
                query.Append(" ,password = @password");
                parameters.Add("@password", user.Password);

            }
            query.Append("WHERE user_id = @user_id");
            
            
            
            var res = await fetcher.GetTableByParameter(query.ToString(), parameters); 
            return res.Rows.Count > 0 ? user : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating user", e);
        }
    }

    public async Task<User?> DeleteUserAsync(int id)
    {
        try
        {
            var query = "DELETE FROM users WHERE user_id = @id Returning user_id";
            var table = await fetcher.GetTableByParameter(query, "@id", id);
            return table.Rows.Count > 0 ? new User() : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while deleting user", e);
        }
    }
}