using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class PicturesRepository(
    IConfiguration configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.GetConnectionString("UserDbConnection")
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    public async Task<Picture> GetProfilePictureAsync(long? id)
    {
        if (id == null) return null;
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT * FROM pictures" +
                                                                     " WHERE picture_id = @id", "@id", (long)id);
        if (table.Rows.Count == 0) return null;
        return entityCreator.CreatePicture(table.Rows[0]);
    }
    
    public async Task<IEnumerable<Picture>> GetPicturesByUserIdAsync(long id)
    {
        var output = new List<Picture>();
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT * FROM pictures" +
                                                                     " WHERE user_id = @id", "@id", id);
        foreach (DataRow row in table.Rows)
        {
            output.Add(entityCreator.CreatePicture(row));
        }
        
        return output;
    }

    public void UploadPhoto(long id, byte[] filePicture, long isMain)
    {
        var query = new StringBuilder("INSERT INTO pictures (user_id, picture_path, is_profile_picture) VALUES (@userId, @picture, @isMain)");
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.Parameters.AddWithValue("@userId", id);
        command.Parameters.AddWithValue("@picture", filePicture);
        command.Parameters.AddWithValue("@isMain", isMain);
        command.CommandText = query.ToString();
        command.ExecuteNonQuery();
    }
}