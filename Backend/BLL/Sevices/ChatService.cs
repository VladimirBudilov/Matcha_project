using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class ChatService(
    RoomsRepository roomsRepository,
    MessagesRepository messagesRepository
    )
{
    public async Task<List<Message>> GetMessages(int producerId, int consumerId)
    {
        var room = await roomsRepository.GetRoom(producerId, consumerId);
        var producerMessages = await messagesRepository.GetMessages(room, producerId);
        var consumerMessages = await messagesRepository.GetMessages(room, consumerId);
        var messages = producerMessages.Concat(consumerMessages).ToList();
        return messages;
    }
    
    public async Task<Message> AddMessage(Message message)
    {
        return await messagesRepository.AddMessage(message);
    }
}