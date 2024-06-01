using System.Collections.Concurrent;
using AutoMapper.Internal;
using DAL.Helpers;
using DAL.Repositories;

namespace Web_API.Hubs.Services;

public class ChatManager(
    RoomsRepository roomsRepository,
    MessagesRepository messagesRepository,
    LikesRepository likesRepository,
    TableFetcher tableFetcher)
{
    
    public ConcurrentDictionary<int, string[]> Rooms { get; set; } = new();
    
    public async Task<bool> CanChat(int inviterId, int inviteeId)
    {
        var inviterHasLike = await likesRepository.HasLike(inviterId, inviteeId);
        var inviteeHasLike = await likesRepository.HasLike(inviteeId, inviterId);
        return inviterHasLike && inviteeHasLike;
    }

    public async Task<int> GetRoomName(int inviterId, int inviteeId)
    {
        var roomId = await roomsRepository.GetRoom(inviterId, inviteeId);
        if (roomId != 0) return roomId;
        return await roomsRepository.CreateRoom(inviterId, inviteeId);
    }

    public void ConnectToRoom(int room, string contextConnectionId)
    {
        
    }
}