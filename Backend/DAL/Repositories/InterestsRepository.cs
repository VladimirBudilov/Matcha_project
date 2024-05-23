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

    public async Task<Interest> CreateInterestAsync(string entity)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append(
            $"INSERT INTO interests (name) VALUES (\'{entity}\') RETURNING interest_id");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        var id = await command.ExecuteScalarAsync();
        var interest = new Interest
        {
            Name = entity,
            InterestId = (int)id
        };
        return interest;
    }


    public async Task<IEnumerable<Interest>> GetInterestsByUserIdAsync(int id)
    {
        var output = new List<Interest>();
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var table = await fetcher.GetTableByParameter(connection,
            "SELECT interests.name, interests.interest_id FROM interests" +
            " JOIN user_interests ON interests.interest_id = user_interests.interest_id" +
            " WHERE user_interests.user_id = @id", "@id", id);
        foreach (DataRow row in table.Rows)
        {
            var interest = entityCreator.CreateInterests(row);
            output.Add(interest);
        }

        return output;
    }

    public async Task UpdateUserInterestsAsync(int userId, List<Interest> profileInterests)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("DELETE FROM user_interests WHERE user_id = @id");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        command.Parameters.AddWithValue("@id", userId);
        await command.ExecuteNonQueryAsync();
        if(profileInterests.Count == 0) return;
        
        query = new StringBuilder("INSERT INTO user_interests (user_id, interest_id) VALUES ");
        var parameters = new Dictionary<string, object>();

        for (int i = 0; i < profileInterests.Count; i++)
        {
            var interest = profileInterests[i];
            query.Append($"(@userId{i}, @interestId{i}),");
            parameters.Add($"@userId{i}", userId);
            parameters.Add($"@interestId{i}", interest.InterestId);
        }

// Remove the last comma and finalize the query
        query.Length--;
        query.Append(";");

// Inject parameters and execute the query
        injector.InjectParameters(query, parameters);
        command.CommandText = query.ToString();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Interest>> GetUserInterestsByNamesAsync(IEnumerable<string> select)
    {
        if (select.Count() == 0) return new List<Interest>();
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("SELECT name, interests.interest_id FROM interests");
        query.Append(" WHERE interests.name IN (");
        foreach (var interest in select)
        {
            query.Append($" '{interest}',");
        }

        query.Remove(query.Length - 1, 1);
        query.Append(")");
        var table = await fetcher.GetTableAsync(connection, query.ToString());

        return (from DataRow row in table.Rows select entityCreator.CreateInterests(row)).ToList();
    }

    public async Task DeleteInterestByNAmeAsync(string interest)
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

    public async Task DeleteInterestByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        connection.CreateCommand();
        var query = new StringBuilder().Append("DELETE FROM interests WHERE interest_id = @id");
        var command = connection.CreateCommand();
        command.CommandText = query.ToString();
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }
}