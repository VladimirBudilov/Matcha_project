using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;

namespace Web_API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub(
    ChatManager chatManager,
    ActionService actionService,
    ILogger<ChatHub> logger,
    ClaimsService claimsService,
    UserService userService,
    ChatService chatService,
    NotificationService notificationService
    
) : Hub<IChat>
{
    public override Task OnConnectedAsync()
    {
        var user = claimsService.GetId(Context.User?.Claims);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = claimsService.GetId(Context.User?.Claims);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(int inviteeId, string message)
    {
        var inviterId = claimsService.GetId(Context.User?.Claims);
        var blocked = await actionService.CheckIfUserIsBlocked(inviterId, inviteeId);
        if(blocked) return;
        var roomName = await chatManager.GetRoomName(inviterId, inviteeId);
        var user = await userService.GetUserByIdAsync(inviterId);
        await chatService.AddMessage(inviterId, roomName, message);
        notificationService.AddNotification(inviteeId, new Notification()
        {
            Actor = user.FirstName + " " + user.LastName,
            Message = "Sent you a message",
            Type = NotificationType.ChatMessage
        });
        await Clients.Group(roomName.ToString()).ReceiveMessage(user!.FirstName, message);
        await notificationService.SendNotificationToUser(inviteeId);
    }
    
    public async Task StartChat(int inviteeId)
    {
        var inviterId = claimsService.GetId(Context.User?.Claims);
        if(!await chatManager.CanChat(inviterId, inviteeId)) return;
        var roomName = await chatManager.GetRoomName(inviterId, inviteeId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName.ToString());
    }

    public async Task LeaveChat(int inviteeId)
    {
        var inviterId = claimsService.GetId(Context.User?.Claims);
        var roomName = await chatManager.GetRoomName(inviterId, inviteeId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName.ToString());
    }
}
