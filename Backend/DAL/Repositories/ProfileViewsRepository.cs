using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class ProfileViewsRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{

    public async Task<ProfileView> CreateProfileViewsAsync(ProfileView entity)
    {
        var query = new StringBuilder()
            .Append("INSERT INTO profile_views (profile_user_id, viewer_user_id)")
            .Append("VALUES (@profile_user_id, @viewer_user_id)");
        var parameters = new Dictionary<string, object>
        {
            {"@profile_user_id", entity.ViewedId},
            {"@viewer_user_id", entity.ViewerId}
        };
        await fetcher.GetTableByParameter(query.ToString(), parameters);
        return entity;
    }

    public async Task<IEnumerable<ProfileView>> GetProfileViewsByUserIdAsync(int likedId)
    {

        var query = "SELECT * FROM profile_views WHERE profile_user_id = @id";
        var table = await fetcher.GetTableByParameter(query, "@id", likedId);

        return (from DataRow row in table.Rows select entityCreator.CreateProfileViews(row)).ToList();


    }

    public async Task<ProfileView?> GetView(int viewerId, int viewedId)
    {
        var query = new StringBuilder()
            .Append("SELECT * FROM profile_views ")
            .Append(" WHERE profile_user_id = @profile_user_id AND viewer_user_id = @viewer_user_id");
        var table = await fetcher.GetTableByParameter(query.ToString(), new Dictionary<string, object>
        {
            {"@profile_user_id", viewedId},
            {"@viewer_user_id", viewerId}
        });
        
        return table.Rows.Count == 0 ? null : entityCreator.CreateProfileViews(table.Rows[0]);
    }
}