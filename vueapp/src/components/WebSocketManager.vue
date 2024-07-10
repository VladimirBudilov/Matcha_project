<template>
  <!-- This component does not render anything -->
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted } from 'vue';
import createConnection from '@/services/NotificationService';
import {Notification, useNotificationStore} from '@/stores/NotoficationStore'
import { notification } from 'ant-design-vue';

const store = useNotificationStore();

onMounted(async () => {
  if(!localStorage['token']) return;
  if (!store.connection) {
    const connection = createConnection();
    if (connection) {
        await connection.start();
        connection.on('ReceiveNotifications', function (GetNotifications: []) {
          GetNotifications.forEach((GetNotification : Notification) => {
            if(GetNotification.type != null)
            {
              notification.info({
                message: GetNotification.actor,
                description: GetNotification.type.toString(),
                duration: 0,
              })
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
