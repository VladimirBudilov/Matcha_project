using System.Data;
using System.Text;
using DAL.Entities;

namespace DAL.Helpers;

public class BlackListRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{
    public async Task<BlackList> AddToBlackListAsync(BlackList entity)
    {

            var query = new StringBuilder()
                .Append("INSERT INTO black_list (user_id, blocked_user_id)")
                .Append(" VALUES (@user_id, @blocked_user_id)");
            var parameters =
                new Dictionary<string, object>
                {
                    { "@user_id", entity.UserId },
                    { "@blocked_user_id", entity.BlacklistedUserId }
                };
            await fetcher.GetTableByParameter(query.ToString(), parameters);
            return entity;

    }

    public async Task<BlackList> DeleteFromBlackListAsync(int userId, int blacklistedUserId)
    {

            var query = new StringBuilder()
                .Append("DELETE FROM black_list WHERE user_id = @user_id")
                .Append(" AND blacklisted_user_id = @blocked_user_id");
            var parameters = new Dictionary<string, object>
            {
                { "@user_id", userId },
                { "@blocked_user_id", blacklistedUserId }
            };
            await fetcher.GetTableByParameter(query.ToString(), parameters);
            return new BlackList { UserId = userId, BlacklistedUserId = blacklistedUserId };
    }
    
    public async Task<IEnumerable<BlackList>> GetFromBlackListByIdAsync(int id)
    {
        var query = new StringBuilder()
            .Append("SELECT * FROM black_list")
            .Append(" WHERE user_id = @user_id");
        var parameters = new Dictionary<string, object>
        {
            { "@user_id", id }
        };
        var table = await fetcher.GetTableByParameter(query.ToString(), parameters);
        return (from DataRow row in table.Rows select entityCreator.CreateBlackList(row)).ToList();
    }
}