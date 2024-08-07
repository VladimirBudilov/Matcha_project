﻿using System.Collections.Concurrent;
using BLL.Sevices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Web_API.Hubs.Helpers;
using Web_API.Hubs.Services;

namespace Web_API.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationHub(
    ILogger<ChatHub> logger,
    ClaimsService claimsService,
    NotificationService notificationService,
    UserService userService
) : Hub<INotificationHub>
{
    public override Task OnConnectedAsync()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        var identifier = Context.ConnectionId!;
        notificationService.AddOnlineUser(userId, identifier);
        userService.UpdateLastLogin(userId).Wait();
        GetNotifications().Wait();
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        notificationService.RemoveOnlineUser(userId);
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task GetNotifications()
    {
        var userId = claimsService.GetId(Context.User?.Claims);
        var notifications = notificationService.GetNotifications(userId);
        if(notifications.Count == 0) return;
        await Clients.Caller.ReceiveNotifications(notifications);
    }
    
    public async Task SendNotificationToUser(int id)
    {
        var identifier = notificationService.GetIdentifier(id);
        if(!notificationService.GetUsersId().Contains(id)) return;
        var userNotifications = notificationService.GetNotifications(id);
        if(userNotifications.Count == 0) return;
        await Clients.Client(identifier).ReceiveNotifications(userNotifications);
        notificationService.ClearNotifications(id);
    }
}
