using System.Data;
using System.Text;
using DAL.Entities;
using DAL.Helpers;

namespace DAL.Repositories;

public class MessagesRepository(
    TableFetcher fetcher,
    EntityCreator entityCreator)
{
    public async Task<Message> AddMessage(Message message)
    {
        var query = new StringBuilder()
            .Append("INSERT INTO messages (room_id, sender_id, message, created_at)")
            .Append("VALUES (@room_id, @sender_id, @text, @created_at)");
        var parameters = new Dictionary<string, object>
        {
            { "@room_id", message.RoomId },
            { "@sender_id", message.SenderId },
            { "@text", message.Content },
            { "@created_at", message.Created_at }
        };
        await fetcher.GetTableByParameterAsync(query.ToString(), parameters);
        return message;
    }
    
    public async Task<List<Message>> GetMessages(int room, int producerId)
    {
        var query = new StringBuilder()
            .Append("SELECT * FROM messages")
            .Append(" WHERE room_id = @room_id AND sender_id != @producer_id");
        var parameters = new Dictionary<string, object>
        {
            { "@room_id", room },
            { "@producer_id", producerId }
        };
        var table = await fetcher.GetTableByParameterAsync(query.ToString(), parameters);
        return (from DataRow row in table.Rows select entityCreator.CreateMessage(row)).ToList();
    }
}