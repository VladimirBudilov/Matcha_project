using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class ProfileViewsRepository(
    DatabaseSettings configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    public async Task<ProfileView> CreateProfileViewsAsync(ProfileView entity)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var query = new StringBuilder().Append(
            "INSERT INTO profile_views (profile_user_id, viewer_user_id) VALUES (@profile_user_id, @viewer_user_id)");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        command.Parameters.AddWithValue("@profile_user_id", entity.ViewedId);
        command.Parameters.AddWithValue("@viewer_user_id", entity.ViewerId);
        await command.ExecuteNonQueryAsync();
        return entity;
    }

    public async Task<IEnumerable<ProfileView>> GetProfileViewsByUserIdAsync(int likedId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var query = new StringBuilder().Append(
            "SELECT * FROM profile_views WHERE profile_user_id = @id");
        var table = await fetcher.GetTableByParameter(connection, query.ToString(), "@id", likedId);

        return (from DataRow row in table.Rows select entityCreator.CreateProfileViews(row)).ToList();


    }
}