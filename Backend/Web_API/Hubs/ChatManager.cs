using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Web_API.Hubs;

public class ChatManager
{
    public ConcurrentDictionary<string, List<string>> Rooms { get; set; } = new();

    public void CreateRoom(string roomName, HubCallerContext context)
    {
        if (Rooms.ContainsKey(roomName))
        {
            throw new Exception("A chat room with this name already exists.");
        }

        Rooms.TryAdd(roomName, new List<string> { context.ConnectionId });
    }

    public async Task InviteToRoom(string roomName, string connectionId, IHubCallerClients<IChat> clients)
    {
        // Check if the room exists
        if (!Rooms.ContainsKey(roomName))
        {
            throw new Exception("The chat room does not exist.");
        }

        // Send an invitation to the user
        await clients.Client(connectionId).ReceiveInvitation(roomName);
    }

    public void JoinRoom(string roomName, HubCallerContext context)
    {
        // Check if the room exists
        if (!Rooms.ContainsKey(roomName))
        {
            throw new Exception("The chat room does not exist.");
        }

        // Add the current user to the room
        Rooms[roomName].Add(context.ConnectionId);
    }

    public void LeaveRoom(string roomName, HubCallerContext context)
    {
        throw new NotImplementedException();
    }
    
    public async Task SendMessageToRoom(string roomName, string message, IHubCallerClients<IChat> Clients, HubCallerContext Context)
    {
        // Check if the room exists
        if (!Rooms.ContainsKey(roomName))
        {
            throw new Exception("The chat room does not exist.");
        }

        // Get the connection ids of the users in the room
        var connectionIds = Rooms[roomName];

        // Send the message to the users in the room
        await Clients.Clients(connectionIds).ReceiveMessage(Context.User.Identity.Name, message);
    }
}