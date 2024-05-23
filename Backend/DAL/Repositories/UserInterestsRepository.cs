using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class UserInterestsRepository(
    DatabaseSettings configuration,
    EntityCreator entityCreator,
    TableFetcher fetcher,
    ParameterInjector injector)
{
    private readonly string _connectionString = configuration.ConnectionString
                                                ?? throw new ArgumentNullException(nameof(configuration),
                                                    "Connection string not found in configuration");
    public async Task<UserInterest> DeleteUserInterestAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var query = new StringBuilder().Append(
            "DELETE FROM user_interests WHERE user_interest_id = @id RETURNING *");
        var table = await fetcher.GetTableByParameter(connection, query.ToString(), "@id", id);
        return entityCreator.CreateUserInterest(table.Rows[0]);
    }
    
    public async Task<IEnumerable<UserInterest>> GetAllUserInterestsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        
        var query = new StringBuilder().Append("SELECT * FROM user_interests");
        var table = await fetcher.GetTableAsync(connection, query.ToString());
        return (from DataRow row in table.Rows select entityCreator.CreateUserInterest(row)).ToList();
    }
}