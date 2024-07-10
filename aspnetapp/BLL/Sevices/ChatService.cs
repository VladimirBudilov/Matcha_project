using DAL.Entities;
using DAL.Repositories;

namespace BLL.Sevices;

public class ChatService(
    RoomsRepository roomsRepository,
    MessagesRepository messagesRepository,
    UsersRepository usersRepository
    )
{
    public async Task<List<Message>> GetMessages(int producerId, int consumerId)
    {
        var room = await roomsRepository.GetRoom(producerId, consumerId);
        var producerMessages = await messagesRepository.GetMessages(room, producerId);
        var producer = await usersRepository.GetUserByIdAsync(producerId);
        producerMessages.ForEach(m => m.Author = producer!.UserName + " " + producer!.LastName);
        var consumerMessages = await messagesRepository.GetMessages(room, consumerId);
        var consumer = await usersRepository.GetUserByIdAsync(consumerId);
        consumerMessages.ForEach(m => m.Author = consumer!.UserName + " " + consumer!.LastName);
        var messages = producerMessages.Concat(consumerMessages).ToList();
        messages.Sort((a, b) => a.Created_at.CompareTo(b.Created_at));
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