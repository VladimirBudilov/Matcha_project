export class NotificationType {
    readonly ChatMessage = "ChatMessage";
    readonly Like = "Like";
    readonly ResponseLike = "ResponseLike";
    readonly Unlike = "Unlike";
    readonly Viewed = "Viewed";
}

export class Notification {
    User: string;
    Message: string;
    Type: NotificationType;

    constructor(user: string, message: string, type: NotificationType) {
        this.User = user;
        this.Message = message;
        this.Type = type;
    }
}