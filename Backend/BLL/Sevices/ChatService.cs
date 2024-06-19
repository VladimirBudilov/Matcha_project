using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class ChatService(
    RoomsRepository roomsRepository,
    MessagesRepository messagesRepository,
    UserRepository userRepository
    )
{
    public async Task<List<Message>> GetMessages(int producerId, int consumerId)
    {
        var room = await roomsRepository.GetRoom(producerId, consumerId);
        var producerMessages = await messagesRepository.GetMessages(room, producerId);
        var consumerMessages = await messagesRepository.GetMessages(room, consumerId);
        var messages = producerMessages.Concat(consumerMessages).ToList();
        messages.Sort((a, b) => a.Created_at.CompareTo(b.Created_at));
        //add Author to each message
        foreach (var message in messages)
        {
            var user = await userRepository.GetUserByIdAsync(message.SenderId);
            message.Author = user.FirstName + " " + user.LastName;
        }
        return messages;
    }
    
    public async Task<Message> AddMessage(int inviterId, int roomName, string text)
    {
        var message = new Message()
        {
            Created_at = DateTime.Now,
            RoomId = roomName,
            SenderId = inviterId,
            Content = text
        };
        return await messagesRepository.AddMessage(message);
    }
}