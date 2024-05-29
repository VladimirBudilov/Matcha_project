using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web_API.Hubs.Services;

namespace Web_API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub(
    ChatManager chatManager,
    ILogger<ChatHub> logger,
    ClaimsService claimsService
) : Hub<IChat>
{
    public override Task OnConnectedAsync()
    {
        var user = claimsService.GetId(Context.User?.Claims);
        logger.LogInformation($"Actor {user} connected to chat");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = claimsService.GetId(Context.User?.Claims);
        logger.LogInformation($"Actor {user} disconnected from chat");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string room, string message)
    {
        var user = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        await chatManager.SendMessageToRoom(room, message, Clients, Context, user);

    }

    public async Task InviteUser(string connectionId, string roomName)
    {
        await Clients.Client(connectionId).ReceiveInvitation(roomName);
    }

    public async Task CreateRoom(string roomName)
    {
        logger.LogInformation($"Actor {claimsService.GetId(Context.User?.Claims)} created room: {roomName}");
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
