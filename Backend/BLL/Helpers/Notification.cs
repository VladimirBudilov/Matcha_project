namespace Web_API.Hubs.Helpers;

public class Notification
{
    public string Actor { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    
}

public class NotificationType
{
    public static readonly string ChatMessage = "ChatMessage";
    public static readonly string Like = "Like";
    public static readonly string ResponseLike = "ResponseLike";
    public static readonly string Unlike = "Unlike";
    public static readonly string View = "View";
    public static readonly string Online = "Online";
}

