using System.Data;
using System.Text;
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

    public async Task<List<Interest>> GetInterestsAsync()
    {
        var output = new List<Interest>();
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableAsync(connection, "SELECT * FROM interests");
        foreach (DataRow row in table.Rows)
        {
            var interest = entityCreator.CreateInterests(row);
            output.Add(interest);
        }

        return output;
    }
    
    public async Task<Interest> CreateInterestAsync(Interest entity)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("INSERT INTO interests (name) VALUES (@name) RETURNING interest_id");
        injector.InjectParameters( query,
            new Dictionary<string, object>
            {
                {"@name", entity.Name}
            });
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        var id = await command.ExecuteScalarAsync();
        entity.InterestId = (int)id;
        return entity;
    }
    

    public async Task<IEnumerable<Interest>> GetInterestsByUserIdAsync(long id)
    {
        var output = new List<Interest>();
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

    public async Task UpdateUserInterestsAsync(int id, List<Interest> profileInterests)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("DELETE FROM user_interests WHERE user_id = @id");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
        foreach (var interest in profileInterests)
        {
            query = new StringBuilder().Append("INSERT INTO user_interests (user_id, interest_id) VALUES (@userId, @interestId)");
            injector.InjectParameters(query,
                new Dictionary<string, object>
                {
                    {"@userId", id},
                    {"@interestId", interest.InterestId}
                });
            command.CommandText = query.ToString();
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Interest>> GetUserInterestsByNamesAsync(int userId, IEnumerable<string> select)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("SELECT * FROM interests WHERE name IN (");
        foreach (var interest in select)
        {
            query.Append($"'{interest}',");
        }

        query.Remove(query.Length - 1, 1);
        query.Append(")");
        query.Append($" AND {userId} = user_id");
        var table = await fetcher.GetTableAsync(connection, query.ToString());

        return (from DataRow row in table.Rows select entityCreator.CreateInterests(row)).ToList();
    }

    public async Task RemoveInterest(string interest)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("DELETE FROM interests WHERE name = @name");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        command.Parameters.AddWithValue("@name", interest);
        await command.ExecuteNonQueryAsync();
    }
}