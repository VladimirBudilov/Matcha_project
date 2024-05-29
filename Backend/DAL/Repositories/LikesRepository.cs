using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class LikesRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{

    public async Task<Like> CreateLikesAsync(Like entity)
    {
        try
        {
            var query = new StringBuilder()
                .Append("INSERT INTO likes (liker_user_id, liked_user_id)")
                .Append(" VALUES (@liker_user_id, @liked_user_id)");
            var parameters =
                new Dictionary<string, object>
                {
                    { "@liker_user_id", entity.LikerId },
                    { "@liked_user_id", entity.LikedId }
                };
            await fetcher.GetTableByParameter(query.ToString(), parameters);
            return entity;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while creating like");
        }
    }


    public async Task<Like> DeleteLikeAsync(int likerId, int likedId)
    {
        try
        {
            var query = new StringBuilder()
                .Append("DELETE FROM likes WHERE liker_user_id = @liker_user_id")
                .Append(" AND liked_user_id = @liked_user_id");
            var parameters = new Dictionary<string, object>
            {
                { "@liker_user_id", likerId },
                { "@liked_user_id", likedId }
            };
            await fetcher.GetTableByParameter(query.ToString(), parameters);
            return new Like { LikerId = likerId, LikedId = likedId };
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while deleting like");
        }
    }


    public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(int id)
    {
        try
        {
            var output = new List<Like>();
            var query = "SELECT * FROM likes WHERE liked_user_id = @id";
            var table = await fetcher.GetTableByParameter(query, "@id", id);
            foreach (DataRow row in table.Rows)
            {
                var like = entityCreator.CreateLikes(row);
                output.Add(like);
            }

            return output;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while fetching likes");
        }
    }

    public async Task<bool> HasLike(int viewerId, int userId)
    {
        try
        {
            var query = "SELECT * FROM likes WHERE liker_user_id = @viewer_id AND liked_user_id = @user_id";
            var table = await fetcher.GetTableByParameter(
                query, new Dictionary<string, object>
                {
                    { "@viewer_id", viewerId },
                    { "@user_id", userId }
                });

            return table.Rows.Count > 0;
        }
        catch (Exception e)
        {
            throw new DataAccessErrorException("Error while fetching likes");
        }
    }

    public async Task<Like> GetLikeAsync(int likerId, int likedId)
    {
        var query = "SELECT * FROM likes WHERE liker_user_id = @liker_user_id AND liked_user_id = @liked_user_id";

        var table = await fetcher.GetTableByParameter(query, new Dictionary<string, object>
        {
            { "@liker_user_id", likerId },
            { "@liked_user_id", likedId }
        });

        var like = new Like() { LikerId = likerId, LikedId = likedId };
        if (table.Rows.Count == 0) return like;
        like.IsLiked = false;
        return like;
    }
}