namespace Web_API.Hubs.Helpers;

public class Notification
{
    public string User { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public class NotificationType
    {
        public readonly string ChatMessage = "ChatMessage";
        public readonly string Like = "Like";
        public readonly string ResponseLike = "ResponseLike";
        public readonly string Unlike = "Unlike";
        public readonly string Viewed = "Viewed";
    }
}

