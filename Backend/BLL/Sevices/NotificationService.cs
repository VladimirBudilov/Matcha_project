using System.Collections.Concurrent;
using Web_API.Hubs.Helpers;

namespace Web_API.Hubs.Services;

public class NotificationService
{
    private ConcurrentDictionary<int, string> UsersOnline { get; set; } = new();
    private ConcurrentDictionary<int, List<Notification>> UsersNotification { get; set; } = new();
    
    public void AddOnlineUser(int userId,string unqueId)
    {
        UsersOnline.TryAdd(userId, unqueId);
    }
    
    public void RemoveOnlineUser(int userId)
    {
        UsersOnline.TryRemove(userId, out _);
    }
    
    public void AddNotification(int userId, Notification notification)
    {
        if (!UsersNotification.ContainsKey(userId))
        {
            UsersNotification.TryAdd(userId, new List<Notification> { notification });
        }
        else
        {
            UsersNotification[userId].Add(notification);
        }
    }
    
    public List<Notification> GetNotifications(int userId)
    {
        return UsersNotification.ContainsKey(userId) ? UsersNotification[userId] : new List<Notification>();
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
        return UsersOnline[userId];
    }
}