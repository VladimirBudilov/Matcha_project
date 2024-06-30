using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Microsoft.Extensions.Primitives;
using Npgsql;


namespace DAL.Repositories;

public sealed class InterestsRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{
    public async Task<List<Interest>> GetInterestsAsync()
    {
        var output = new List<Interest>();
        var table = await fetcher.GetTableAsync("SELECT * FROM interests");
        foreach (DataRow row in table.Rows)
        {
            var interest = entityCreator.CreateInterests(row);
            output.Add(interest);
        }

        return output;
    }

    public async Task<Interest> CreateInterestAsync(string entity)
    {
       
        var query = "INSERT INTO interests (name) VALUES ( @entity ) RETURNING interest_id";
        var parameters = new Dictionary<string, object> {{"@entity", entity}};
        var table = await fetcher.GetTableByParameter(query, parameters);
        var interest = new Interest
        {
            Name = entity,
            InterestId = (int) table.Rows[0]["interest_id"]
        };
        return interest;
    }
    public async Task<Interest> CreateInterestWithIdAsync(int interest_id, string entity)
    {
       
        var query = "INSERT INTO interests (interest_id, name) VALUES (@interest_id, @entity ) RETURNING interest_id";
        var parameters = new Dictionary<string, object>
        {
            {"@entity", entity},
            {"@interest_id", interest_id}
        };
        var table = await fetcher.GetTableByParameter(query, parameters);
        var interest = new Interest
        {
            Name = entity,
            InterestId = (int) table.Rows[0]["interest_id"]
        };
        return interest;
    }


    public async Task<IEnumerable<Interest>> GetInterestsByUserIdAsync(int id)
    {
        var output = new List<Interest>();
        var query = new StringBuilder()
            .Append("SELECT interests.name, interests.interest_id FROM interests")
            .Append(" JOIN user_interests ON interests.interest_id = user_interests.interest_id")
            .Append(" WHERE user_interests.user_id = @id");
        var table = await fetcher.GetTableByParameter(query.ToString(), "@id", id);
        foreach (DataRow row in table.Rows)
        {
            var interest = entityCreator.CreateInterests(row);
            output.Add(interest);
        }

        return output;
    }

    public async Task UpdateUserInterestsAsync(int userId, List<Interest> profileInterests)
    {
        var query = "DELETE FROM user_interests WHERE user_id = @id";
        await fetcher.GetTableByParameter(query, "@id", userId);
        if(profileInterests.Count == 0) return;
        
        var queryBuilder = new StringBuilder("INSERT INTO user_interests (user_id, interest_id) VALUES ");
        var parameters = new Dictionary<string, object>();

        for (int i = 0; i < profileInterests.Count; i++)
        {
            var interest = profileInterests[i];
            queryBuilder.Append($"(@userId{i}, @interestId{i}),");
            parameters.Add($"@userId{i}", userId);
            parameters.Add($"@interestId{i}", interest.InterestId);
        }
        queryBuilder.Length--;
        queryBuilder.Append(';');

        await fetcher.GetTableByParameter(queryBuilder.ToString(), parameters);
    }

    public async Task<List<Interest>> GetUserInterestsByNamesAsync(List<string> select)
    {
        if (select.Count == 0) return new List<Interest>();
        var query = new StringBuilder()
            .Append("SELECT name, interests.interest_id FROM interests")
            .Append(" WHERE interests.name IN (");
        foreach (var interest in select)
        {
            query.Append($" '{interest}',");
        }

        query.Remove(query.Length - 1, 1);
        query.Append(')');
        var table = await fetcher.GetTableAsync(query.ToString());

        return (from DataRow row in table.Rows select entityCreator.CreateInterests(row)).ToList();
    }

    public async Task DeleteInterestByIdAsync(int id)
    {
        var query = "DELETE FROM interests WHERE interest_id = @id";
        await fetcher.GetTableByParameter(query, "@id", id);
    }
}