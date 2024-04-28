using System.Data;
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
    private readonly EntityCreator _entityCreator = entityCreator;
    private readonly TableFetcher _fetcher = fetcher;
    private readonly ParameterInjector _injector = injector;

    public async Task<Picture> GetProfilePictureAsync(long? id)
    {
        if (id == null) return null;
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT * FROM pictures" +
                                                                     " WHERE picture_id = @id", "@id", (long)id);
        if (table.Rows.Count == 0) return null;
        return _entityCreator.CreatePicture(table.Rows[0]);
    }
    
    /*public async Task<Pictures> CreatePicturesAsync(Pictures entity)
    {
        
    }
    
    
    public async Task<Pictures> DeletePicturesAsync(int id)
    {
        
    }*/

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
            output.Add(_entityCreator.CreatePicture(row));
        }
        
        return output;
    }
}