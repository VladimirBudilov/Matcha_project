import {defineStore} from "pinia";
import {type HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {ref} from "vue";
import {useStorage} from "@vueuse/core";
import createConnection from "@/services/NotificationService";

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

export const useNotificationStore = defineStore('notification', {
    state: () => ({
        connection: ref<HubConnection>(),
        notifications: ref<Notification[]>([])
    }),
    actions: {
        setConnection(connection: HubConnection) {
            this.connection = connection;
        },
        addNotification(notification: Notification) {
            this.notifications.push(notification);
        },
        ClearNotifications() {
            this.notifications = [];
        }
    },
    getters: {
        getNotifications() : Notification[]
        {
            return this.notifications;
        },
        getConnection() : HubConnection | undefined
        {
            return this.connection;
        }
    }
});