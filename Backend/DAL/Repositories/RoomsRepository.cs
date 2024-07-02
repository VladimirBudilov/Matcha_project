using System.Text;
using DAL.Helpers;
using Microsoft.Extensions.Primitives;

namespace DAL.Repositories;

public class RoomsRepository(TableFetcher fetcher)
{
    public async Task<int> GetRoom(int inviterId, int invitedId)
    {
        var query = new StringBuilder()
            .Append("SELECT id FROM rooms")
            .Append(" WHERE (user1 = @inviterId AND user2 = @invitedId)")
            .Append(" OR (user1 = @invitedId AND user2 = @inviterId)");
        var parameters = new Dictionary<string, object>
        {
            { "@inviterId", inviterId },
            { "@invitedId", invitedId }
        };
        var table = await fetcher.GetTableByParameterAsync(query.ToString(), parameters);
        return table.Rows.Count == 0 ? 0 : (int)table.Rows[0]["id"];
    }
    
    public async Task<int> CreateRoom(int inviterId, int invitedId)
    {
        var query = new StringBuilder()
            .Append("INSERT INTO rooms (user1, user2)")
            .Append(" VALUES (@inviterId, @invitedId)")
            .Append(" RETURNING id");
        var parameters = new Dictionary<string, object>
        {
            { "@inviterId", inviterId },
            { "@invitedId", invitedId }
        };
        var table = await fetcher.GetTableByParameterAsync(query.ToString(), parameters);
        return (int)table.Rows[0]["id"];
    }
}