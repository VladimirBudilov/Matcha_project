import {defineStore} from "pinia";
import type {HubConnection} from "@microsoft/signalr";
import {ref} from "vue";

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

export const useNotificationStore = defineStore({
    id: 'notification',
    state: () => ({
        notificationConnection: ref<HubConnection | null>(null),
    }),
});