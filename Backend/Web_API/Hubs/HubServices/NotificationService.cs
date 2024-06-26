﻿using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Web_API.Hubs.Helpers;

namespace Web_API.Hubs.Services;

public class NotificationService(IHubContext<NotificationHub, INotificationHub> hubContext)
{
    private ConcurrentDictionary<int, string> UsersOnline { get; set; } = new();
    private ConcurrentDictionary<int, List<Notification>> UsersNotification { get; set; } = new();
    
    public void AddOnlineUser(int userId,string uniqueId)
    {
        UsersOnline[userId] =  uniqueId;
    }
    
    public void RemoveOnlineUser(int userId)
    {
        UsersOnline.TryRemove(userId, out _);
    }
    
    public void AddNotification(int receiverId, Notification notification)
    {
        if (!UsersNotification.ContainsKey(receiverId))
        {
            UsersNotification.TryAdd(receiverId, [notification]);
        }
        else
        {
            UsersNotification[receiverId].Add(notification);
        }
    }
    
    public List<Notification> GetNotifications(int userId)
    {
        return UsersNotification.TryGetValue(userId, out var value) ? value : new List<Notification>();
    }
    
    public void ClearNotifications(int userId)
    {
        if (UsersNotification.ContainsKey(userId))
        {
            UsersNotification[userId].Clear();
        }
    }

    public IEnumerable<int> GetUsersId()
    {
        return UsersOnline.Keys;
    }

    public string GetIdentifier(int userId)
    {
        if (!UsersOnline.ContainsKey(userId)) return string.Empty;
        return UsersOnline[userId];
    }

    public async Task SendNotificationToUser(int userId)
    {
        var identifier = GetIdentifier(userId);
        if(!GetUsersId().Contains(userId)) return;
        var userNotifications = GetNotifications(userId);
        if(userNotifications.Count == 0) return;
        await hubContext.Clients.Client(identifier).ReceiveNotifications(userNotifications);
        ClearNotifications(userId);
    }

    public bool IsUserOnline(int id)
    {
        return UsersOnline.ContainsKey(id);
    }
}