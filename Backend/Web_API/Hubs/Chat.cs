using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web_API.Hubs;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class Chat(ChatManager chatManager,
    ILogger<Chat> logger
    ) : Hub<IChat>
{
    public override Task OnConnectedAsync()
    {
        var user = Context.User?.Claims;
        logger.LogInformation($"User {user} connected");    
        return base.OnConnectedAsync();
    }
        
    public override Task OnDisconnectedAsync(System.Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string room, string message)
    {
        await chatManager.SendMessageToRoom(room, message, Clients, Context);
    }
        
    public async Task InviteUser(string connectionId, string roomName)
    {
        await Clients.Client(connectionId).ReceiveInvitation(roomName);
    }
    
    public async Task CreateRoom(string roomName)
    {
        chatManager.CreateRoom(roomName, Context);
    }
    
    public async Task JoinRoom(string roomName)
    {
        chatManager.JoinRoom(roomName, Context);
    }
    
    public async Task LeaveRoom(string roomName)
    {
        chatManager.LeaveRoom(roomName, Context);
    }
    
    public async Task InviteToRoom(string roomName, string connectionId)
    {
        chatManager.InviteToRoom(roomName, connectionId, Clients);
    }
}