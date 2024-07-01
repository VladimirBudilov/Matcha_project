<template>
  <!-- This component does not render anything -->
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted } from 'vue';
import createConnection from '@/services/NotificationService';
import {Notification, useNotificationStore} from '@/stores/NotoficationStore'
import { message } from 'ant-design-vue';

const store = useNotificationStore();

onMounted(async () => {
  if(!localStorage['token']) return;
  if (!store.connection) {
    const connection = createConnection();
    if (connection) {
        await connection.start();
        connection.on('ReceiveNotifications', function (notifications: []) {
          notifications.forEach((notification : Notification) => {
            if(notification.type != null)
            {
              let content = notification.type + ' by ' + notification.actor;
              message.success(content);
            }
          });
        });
        let id: number = Number(localStorage.getItem('UserId'));
        await connection.invoke("SendNotificationToUser", Number(id));
        store.connection = connection;
      }
    }
    else {
      message.error('Connection is not created');
    }
});
</script>
