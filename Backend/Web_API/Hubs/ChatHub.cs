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
        //check that you should beconnected to chats
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = claimsService.GetId(Context.User?.Claims);
        logger.LogInformation($"Actor {user} disconnected from chat");
        //check that you should be disconnected from chats
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string room, string message)
    {
        var user = Context.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
    
    public async Task StartChat(int inviteeId)
    {
        //check that user can start chat
        var inviterId = claimsService.GetId(Context.User?.Claims);
        if(!await chatManager.CanChat(inviterId, inviteeId)) return;
        //check that room exists
        var room = await chatManager.GetRoom(inviterId, inviteeId);
        //connect users to room
        chatManager.ConnectToRoom(room, Context.ConnectionId);
    }

    public async Task LeaveChat(string roomName)
    {
        //chatManager.LeaveRoom(roomName, Context);
    }
}
