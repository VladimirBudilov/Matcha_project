﻿using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class ProfileRepository(
    IConfiguration configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.GetConnectionString("UserDbConnection")
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");
    private readonly EntityCreator _entityCreator = entityCreator;
    private readonly TableFetcher _fetcher = fetcher;
    private readonly ParameterInjector _injector = injector;

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
            var user = _entityCreator.CreateUser(userInfoRow);
            user.Profile = _entityCreator.CreateUserProfile(userInfoRow);
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
            var dataTable = await _fetcher.GetTableByParameter(connection,
                "SELECT * FROM profiles WHERE profile_id = @id", "@id", id);
            return dataTable.Rows.Count > 0 ? _entityCreator.CreateUserProfile(dataTable.Rows[0]) : null;
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
            "UPDATE profiles SET gender = @gender, sexual_preferences = @sexual_preferences, biography = @biography,  updated_at = @updated_at, location = @location, age = @age WHERE profile_id = @profile_id";
        
        command.Parameters.AddWithValue("@profile_id", entity.ProfileId);
        command.Parameters.AddWithValue("@gender", entity.Gender);
        command.Parameters.AddWithValue("@sexual_preferences", entity.SexualPreferences);
        command.Parameters.AddWithValue("@biography", entity.Biography);
        command.Parameters.AddWithValue("@updated_at", entity.UpdatedAt.ToString());
        command.Parameters.AddWithValue("@location", entity.Location);
        command.Parameters.AddWithValue("@age", entity.Age);

        var res = await command.ExecuteNonQueryAsync();
        return res > 0 ? entity : null;
    }
    catch (Exception e)
    {
        throw new DataAccessErrorException("Error while updating profile", e);
    }
}
}