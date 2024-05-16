using System.Data;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;


namespace DAL.Repositories;

public sealed class InterestsRepository(
    DatabaseSettings configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");

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
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection, "SELECT interests.name FROM interests"+
                                                                  " JOIN user_interests ON interests.interest_id = user_interests.interest_id"+
                                                                  " WHERE user_interests.user_id = @id", "@id", id);
        foreach (DataRow row in table.Rows)
        {
            var interest = entityCreator.CreateInterests(row);
            output.Add(interest);
        }
        
        return output;
    }
}