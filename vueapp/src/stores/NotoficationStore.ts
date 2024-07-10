import {defineStore} from "pinia";
import {type HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {ref} from "vue";
import {useStorage} from "@vueuse/core";
import createConnection from "@/services/NotificationService";

export class NotificationType {
    readonly chatMessage = "ChatMessage";
    readonly like = "Like";
    readonly responseLike = "ResponseLike";
    readonly unlike = "Unlike";
    readonly viewed = "Viewed";
}

export class Notification {
    actor: string;
    message: string;
    type: NotificationType;

    constructor(user: string, message: string, type: NotificationType) {
        this.actor = user;
        this.message = message;
        this.type = type;
    }
}

export const useNotificationStore = defineStore('notification', {
    state: () => ({
        connection: ref<HubConnection | null>(null),
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
        }
    }
});