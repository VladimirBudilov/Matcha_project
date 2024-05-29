using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;
using Npgsql;

namespace DAL.Repositories;

public class UserInterestsRepository(
    EntityCreator entityCreator,
    TableFetcher fetcher)
{
    public async Task<UserInterest> DeleteUserInterestAsync(int id)
    {
        
        var query = "DELETE FROM user_interests WHERE user_interest_id = @id RETURNING *";
        var table = await fetcher.GetTableByParameter(query, "@id", id);
        return entityCreator.CreateUserInterest(table.Rows[0]);
    }
    
    public async Task<IEnumerable<UserInterest>> GetAllUserInterestsAsync()
    {
        var query = "SELECT * FROM user_interests";
        var table = await fetcher.GetTableAsync( query);
        return (from DataRow row in table.Rows select entityCreator.CreateUserInterest(row)).ToList();
    }
}