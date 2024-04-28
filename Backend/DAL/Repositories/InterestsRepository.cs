using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace DAL.Repositories;

public sealed class InterestsRepository(
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

    public async Task<Interests> GetInterestsAsync()
    {
        throw new NotImplementedException();
    }
    
    public async Task<Interests> CreateInterestsAsync(Interests entity)
    {
        throw new NotImplementedException();
    }
    
    public async Task<Interests> UpdateInterestsAsync(Interests entity)
    {
        throw new NotImplementedException();
    }
    
    public async Task<IEnumerable<Interests>> GetUserInterestsByUserIdAsync(long id)
    {
        var output = new List<Interests>();
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT interests.name FROM interests"+
                                                                  " JOIN user_interests ON interests.interest_id = user_interests.user_id"+
                                                                  " WHERE user_interests.user_id = @id", "@id", id);
        foreach (DataRow row in table.Rows)
        {
            var interest = _entityCreator.CreateInterests(row);
            output.Add(interest);
        }
        
        return output;
    }
}