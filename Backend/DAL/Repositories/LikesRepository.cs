using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class LikesRepository(
    DatabaseSettings configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

    public async Task<Like> CreateLikesAsync(Like entity)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            connection.CreateCommand();
            var query = new StringBuilder().Append(
                "INSERT INTO likes (liker_user_id, liked_user_id) VALUES (@liker_user_id, @liked_user_id)");
            injector.InjectParameters(query,
                new Dictionary<string, object>
                    { { "@liker_user_id", entity.LikerId }, { "@liked_user_id", entity.LikedId } });
            var command = connection.CreateCommand();
            command.CommandText = query.ToString();
            await command.ExecuteNonQueryAsync();
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            connection.CreateCommand();
            var query = new StringBuilder().Append(
                "DELETE FROM likes WHERE liker_user_id = @liker_user_id AND liked_user_id = @liked_user_id");
            injector.InjectParameters(query,
                new Dictionary<string, object> { { "@liker_user_id", likerId }, { "@liked_user_id", likedId } });
            var command = connection.CreateCommand();
            command.CommandText = query.ToString();
            await command.ExecuteNonQueryAsync();
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            connection.CreateCommand();
            var query = "SELECT * FROM likes WHERE liked_user_id = @id";
            var table = await fetcher.GetTableByParameter(connection, query, "@id", id);
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
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            connection.CreateCommand();
            var query = "SELECT * FROM likes WHERE liker_user_id = @viewer_id AND liked_user_id = @user_id";
            var table = await fetcher.GetTableByParameter(connection,
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
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = "SELECT * FROM likes WHERE liker_user_id = @liker_user_id AND liked_user_id = @liked_user_id";
        
        var table = await fetcher.GetTableByParameter(connection, query, new Dictionary<string, object>
        {
            { "@liker_user_id", likerId },
            { "@liked_user_id", likedId }
        });
        
        var like =  new Like(){LikerId = likerId, LikedId = likedId};
        if (table.Rows.Count == 0) return like;
        like.IsLiked = false;
        return like;
    }
}