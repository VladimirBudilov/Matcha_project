using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class PicturesRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{
    public async Task<Picture> GetProfileAsync(int? id)
    {
        if (id == null) return null;
        var query = "SELECT * FROM pictures WHERE picture_id = @id";
        var table = await fetcher.GetTableByParameterAsync(query, "@id", (int)id);
        if (table.Rows.Count == 0) return null;
        return entityCreator.CreatePicture(table.Rows[0]);
    }

    public async Task<IEnumerable<Picture>> GetPicturesByUserIdAsync(int userId)
    {
        var query = "SELECT * FROM pictures WHERE user_id = @userId AND is_profile_picture = 0";
        var table = await fetcher.GetTableByParameterAsync(query, "@userId", userId);

        return (from DataRow row in table.Rows select entityCreator.CreatePicture(row)).ToList();
    }
    
    public async Task<long> GetAmountOfPicturesAsync(int userId)
    {
        var query = "SELECT COUNT(*) FROM pictures WHERE user_id = @userId AND is_profile_picture = 0";
        var table = await fetcher.GetTableByParameterAsync(query, "@userId", userId);
        return (long)table.Rows[0]["count"];
    }

    public async Task<int> UploadPhoto(int userId, byte[] filePicture, int isMain)
    {
        var query = 
            "INSERT INTO pictures (user_id, picture_path, is_profile_picture)" +
            " VALUES (@userId, @picture, @isMain)" +
            "RETURNING picture_id;";
        var table = await fetcher.GetTableByParameterAsync(query, new Dictionary<string, object>()
        {
            { "@userId", userId },
            { "@picture", filePicture },
            { "@isMain", isMain }

        });
        return (int)table.Rows[0]["picture_id"];
    }
    public async Task<int> UploadWithIdPhoto(int picture_id, int userId, byte[] filePicture, int isMain)
    {
        var query = 
            "INSERT INTO pictures (picture_id, user_id, picture_path, is_profile_picture)" +
            " VALUES (@picture_id, @userId, @picture, @isMain)" +
            "RETURNING picture_id;";
        var table = await fetcher.GetTableByParameterAsync(query, new Dictionary<string, object>()
        {
            { "@userId", userId },
            { "@picture", filePicture },
            { "@isMain", isMain },
            { "@picture_id", picture_id }

        });
        return (int)table.Rows[0]["picture_id"];
    }

    public async Task DeletePhotoAsync(int userId, int photoId)
    {
        var query = "DELETE FROM pictures WHERE user_id = @userId AND picture_id = @photoId";
        await fetcher.GetTableByParameterAsync(query,
            new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@photoId", photoId }
            });
    }
}