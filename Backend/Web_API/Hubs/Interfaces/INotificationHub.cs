using Web_API.Hubs.Helpers;

namespace Web_API.Hubs;

public interface INotificationHub
{
    Task ReceiveNotifications(List<Notification> notifications);
}