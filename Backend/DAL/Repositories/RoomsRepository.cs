using System.Text;
using DAL.Helpers;
using Microsoft.Extensions.Primitives;

namespace DAL.Repositories;

public class RoomsRepository(
    DatabaseSettings databaseSettings,
    TableFetcher fetcher)
{
    public async Task<int> GetRoom(int producerId, int consumerId)
    {
        var query = new StringBuilder()
            .Append("SELECT id FROM rooms")
            .Append(" WHERE (user1 = @producerId AND user2 = @consumerId)")
            .Append(" OR (user1 = @consumerId AND user2 = @producerId)");
        var parameters = new Dictionary<string, object>
        {
            { "@producerId", producerId },
            { "@consumerId", consumerId }
        };
        var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
        return table.Rows.Count == 0 ? 0 : (int)table.Rows[0]["id"];
    }
}