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
  console.log("trying to connect socket")
  if(!localStorage['token']) return;
  if (!store.connection) {
    const connection = createConnection();
    if (connection) {

        await connection.start();
        connection.on('ReceiveNotifications', function (notifications: []) {
          console.log(notifications)
          notifications.forEach((notification : Notification) => {
            let content = notification.type + ' by ' + notification.actor;
            message.success(content);
          });
        });
        let id: number = Number(localStorage.getItem('UserId'));
        await connection.invoke("SendNotificationToUser", Number(id));
        store.connection = connection;
        console.log("connection updated", store.connection);
      }
    }
    else {
      console.error('Connection is not created');
    }
});
</script>