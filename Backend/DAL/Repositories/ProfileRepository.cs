﻿using System.Data;
using System.Data.SQLite;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class ProfileRepository(
    IConfiguration configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    QueryBuilder queryBuilder)
{
    private readonly string _connectionString = configuration.GetConnectionString("UserDbConnection")
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    public async Task<User?> GetFullProfileAsync(long id)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var userInfo = await fetcher.GetTableByParameter(connection, "SELECT * FROM users" +
                                                                     " LEFT JOIN profiles ON users.user_id = profiles.profile_id" +
                                                                     " WHERE user_id = @id", "@id", id);
        if (userInfo.Rows.Count > 0)
        {
            var userInfoRow = userInfo.Rows[0];
            var user = entityCreator.CreateUser(userInfoRow);
            user.Profile = entityCreator.CreateUserProfile(userInfoRow);
            return user;
        }

        return null;
    }

    public async Task<Profile> GetProfileByIdAsync(long id)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var dataTable = await fetcher.GetTableByParameter(connection,
                "SELECT * FROM profiles WHERE profile_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUserProfile(dataTable.Rows[0]) : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting profile by id", e);
        }
    }

    public async Task<Profile?> CreateProfileAsync(Profile entity)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO profiles (profile_id, created_at, updated_at, gender, age) " +
                " VALUES (@profile_id, @created_at, @updated_at, @gender, @age)";
            command.Parameters.AddWithValue("@profile_id", entity.ProfileId);
            command.Parameters.AddWithValue("@created_at", entity.CreatedAt.ToString());
            command.Parameters.AddWithValue("@updated_at", entity.UpdatedAt.ToString());
            command.Parameters.AddWithValue("@gender", entity.Gender);
            command.Parameters.AddWithValue("@age", entity.Age);
            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while adding profile", e);
        }
    }

    public async Task<Profile> UpdateProfileAsync(Profile entity)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE profiles SET gender = @gender, sexual_preferences = @sexual_preferences, biography = @biography,  updated_at = @updated_at, age = @age, " +
                "latitude = @latitude, longitude = @longitude" +
                " WHERE profile_id = @profile_id";

            command.Parameters.AddWithValue("@profile_id", entity.ProfileId);
            command.Parameters.AddWithValue("@gender", entity.Gender);
            command.Parameters.AddWithValue("@sexual_preferences", entity.SexualPreferences);
            command.Parameters.AddWithValue("@biography", entity.Biography);
            command.Parameters.AddWithValue("@updated_at", entity.UpdatedAt.ToString());
            command.Parameters.AddWithValue("@age", entity.Age);
            command.Parameters.AddWithValue("@latitude", entity.Latitude);
            command.Parameters.AddWithValue("@longitude", entity.Longitude);

            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating profile", e);
        }
    }
    
    //update location
    public async Task<Profile> UpdateLocationAsync(Profile entity)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText =
                "UPDATE profiles SET longitude = @longitude,   = @latitude WHERE profile_id = @profile_id";

            command.Parameters.AddWithValue("@latitude", entity.Latitude);
            command.Parameters.AddWithValue("@longitude", entity.Longitude);
            command.Parameters.AddWithValue("@updated_at", entity.UpdatedAt.ToString());

            var res = await command.ExecuteNonQueryAsync();
            return res > 0 ? entity : null;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while updating location", e);
        }
    }

    public async Task<IEnumerable<User>> GetFullProfilesAsync(SearchParameters searchParams, SortParameters sortParams,
        PaginationParameters pagination)
    {
        /*try
        {*/
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            queryBuilder.Select(
                " users.user_id as user_id, users.*, profiles.* ");
            queryBuilder.From(" users \n JOIN profiles ON users.user_id = profiles.profile_id ");
            queryBuilder.From(" LEFT JOIN user_interests ON user_interests.user_id = users.user_id ");
            queryBuilder.From(" LEFT JOIN interests ON user_interests.interest_id = interests.interest_id ");
            if (searchParams != null)
            {
                 
            }
            if (sortParams?.MainParameter != null)
            {
                
            }

            queryBuilder.GroupBy(" users.user_id ");
            queryBuilder.OrderBy(" fame_rating ASC, count(interests.name) DESC, age, fame_rating DESC ");
            
            command.CommandText = queryBuilder.Build();
            var dataTable = new DataTable();
            var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            return dataTable.Rows.Count > 0 ? entityCreator.CreateUsers(dataTable) : null;            
        /*}
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while getting full profiles", e);
        }*/

        return null;
    }
}