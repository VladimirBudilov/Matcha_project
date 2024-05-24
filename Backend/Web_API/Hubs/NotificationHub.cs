using System.Collections.Concurrent;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;

namespace Web_API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationHub(
    ChatManager chatManager,
    ILogger<ChatHub> logger,
    ClaimsService claimsService,
    NotificationService notificationService
) : Hub<INotificationHub>
{
    public override Task OnConnectedAsync()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        var identifier = Context.UserIdentifier!;
        notificationService.AddOnlineUser(userId, identifier);
        logger.LogInformation($"User {userId} online");
        SendNotificationToUser(userId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        notificationService.RemoveOnlineUser(userId);
        logger.LogInformation($"User {userId} offline");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task GetNotifications()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        var notifications = notificationService.GetNotifications(userId);
        await Clients.Caller.ReceiveNotifications(notifications);
    }
    
    public async Task SendNotificationToUser(int userId)
    {
        var identifier = notificationService.GetIdentifier(userId);
        var userNotifications = notificationService.GetNotifications(userId);
        await Clients.Client(identifier).ReceiveNotifications(userNotifications);
    }
    
    public async Task ClearNotifications()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        notificationService.ClearNotifications(userId);
        await Clients.Caller.ReceiveNotifications(new List<Notification>());
    }
}

public interface INotificationHub
{
    Task ReceiveNotifications(List<Notification> notifications);
}
