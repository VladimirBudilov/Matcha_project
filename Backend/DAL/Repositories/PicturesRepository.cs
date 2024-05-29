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
    public async Task<Picture> GetProfilePictureAsync(int? id)
    {
        if (id == null) return null;
        var query = "SELECT * FROM pictures WHERE user_id = @id AND is_profile_picture = 1";
        var table = await fetcher.GetTableByParameter(query, "@id", (int)id);
        if (table.Rows.Count == 0) return null;
        return entityCreator.CreatePicture(table.Rows[0]);
    }

    public async Task<IEnumerable<Picture>> GetPicturesByUserIdAsync(int id)
    {
        var query = "SELECT * FROM pictures WHERE user_id = @id";
        var table = await fetcher.GetTableByParameter(query, "@id", id);

        return (from DataRow row in table.Rows select entityCreator.CreatePicture(row)).ToList();
    }

    public async Task<int> UploadPhoto(int id, byte[] filePicture, int isMain)
    {
        var query = 
            "INSERT INTO pictures (user_id, picture_path, is_profile_picture)" +
            " VALUES (@userId, @picture, @isMain)" +
            "RETURNING picture_id;";
        var table = await fetcher.GetTableByParameter(query, new Dictionary<string, object>()
        {
            { "@userId", id },
            { "@picture", filePicture },
            { "@isMain", isMain }

        });
        return (int)table.Rows[0]["picture_id"];
    }

    public async Task DeletePhotoAsync(int userId, int photoId)
    {
        var query = "DELETE FROM pictures WHERE user_id = @userId AND picture_id = @photoId";
        await fetcher.GetTableByParameter(query,
            new Dictionary<string, object>
            {
                { "@userId", userId },
                { "@photoId", photoId }
            });
    }
}