using System.Collections.Concurrent;
using AutoMapper.Internal;
using Microsoft.AspNetCore.SignalR;

namespace Web_API.Hubs.Services;

public class ChatManager
{
    public ConcurrentDictionary<string, List<string>> Rooms { get; set; } = new();

    public string CreateRoom(string roomName, HubCallerContext context)
    {
        string output = "room created successfully."; 
        if (Rooms.ContainsKey(roomName))
        {
            output = "A chat room with this name already exists.";
        }

        if(!Rooms.TryAdd(roomName, new List<string> { context.ConnectionId })) output = "An error occurred while creating the chat room.";
        return output;
    }

    public async Task<string> InviteToRoom(string roomName, string connectionId, IHubCallerClients<IChat> clients)
    {
        var output = "Invitation sent successfully.";
        if (!Rooms.ContainsKey(roomName)) output = "The chat room does not exist.";
        
        await clients.Client(connectionId).ReceiveInvitation(roomName);
        return output;
    }

    public string JoinRoom(string roomName, HubCallerContext context)
    {
        var output = "Joined the chat room successfully.";
        if (!Rooms.ContainsKey(roomName)) output = "The chat room does not exist.";
        
        if(!Rooms[roomName].TryAdd(context.ConnectionId)) output = "An error occurred while joining the chat room.";
        return output;
    }

    public void LeaveRoom(string roomName, HubCallerContext context)
    {
        throw new NotImplementedException();
    }
    
    public async Task SendMessageToRoom(string roomName, string message, IHubCallerClients<IChat> Clients,
        HubCallerContext context, string user)
    {
        if (!Rooms.ContainsKey(roomName))
        {
            throw new Exception("The chat room does not exist.");
        }

        var connectionIds = Rooms[roomName];
        
        await Clients.Clients(connectionIds).ReceiveMessage(user.ToString() ?? "anonimus", message);
    }
}