using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Web_API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class Chat(
    ChatManager chatManager,
    ILogger<Chat> logger,
    ClaimsService claimsService
) : Hub<IChat>
{
    public override Task OnConnectedAsync()
    {
        var user = claimsService.GetId(Context.User?.Claims);
        logger.LogInformation($"User {user} connected");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = claimsService.GetId(Context.User?.Claims);
        logger.LogInformation($"User {user} disconnected");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string room, string message)
    {
        //var user = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.sub)?.Value;
        //await chatManager.SendMessageToRoom(room, message, Clients, Context, user);

    }

    public async Task InviteUser(string connectionId, string roomName)
    {
        await Clients.Client(connectionId).ReceiveInvitation(roomName);
    }

    public async Task CreateRoom(string roomName)
    {
        logger.LogInformation($"User {claimsService.GetId(Context.User?.Claims)} created room: {roomName}");
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
