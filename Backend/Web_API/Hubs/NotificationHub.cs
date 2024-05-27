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
        var identifier = Context.ConnectionId!;
        notificationService.AddOnlineUser(userId, identifier);
        logger.LogInformation($"Actor {userId} online");
        GetNotifications().Wait();
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        notificationService.RemoveOnlineUser(userId);
        logger.LogInformation($"Actor {userId} offline");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task GetNotifications()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        var notifications = notificationService.GetNotifications(userId);
        await Clients.Caller.ReceiveNotifications(notifications);
    }
    
    public async Task SendNotificationToUser(int id)
    {
        var identifier = notificationService.GetIdentifier(id);
        if(!notificationService.GetUsersId().Contains(id)) return;
        var userNotifications = notificationService.GetNotifications(id);
        await Clients.Client(identifier).ReceiveNotifications(userNotifications);
    }
    
    public Task ClearNotifications()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        notificationService.ClearNotifications(userId);
        return Task.CompletedTask;
    }
}

public interface INotificationHub
{
    Task ReceiveNotifications(List<Notification> notifications);
}
