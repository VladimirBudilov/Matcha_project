using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public class LikesRepository(
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
   
    public async Task<Like> GetLikesByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Like> CreateLikesAsync(Like entity)
    {
        throw new NotImplementedException();
    }

    
    public async Task<Like> DeleteLikesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Like>> GetLikesByUserIdAsync(long id)
    {
        var output = new List<Like>();
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT likes.*, users.user_name FROM likes"+
                                                                  " JOIN users ON likes.liked_user_id = users.user_id"+
                                                                  " WHERE liked_user_id = @id", "@id", id);
        foreach (DataRow row in table.Rows)
        {
            var like = _entityCreator.CreateLikes(row);
            output.Add(like);
        }
        
        return output;
    }
}