<template>
  <!-- This component does not render anything -->
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted } from 'vue';
import createConnection from '@/services/NotificationService';
import {useNotificationStore} from '@/stores/NotoficationStore'

const store = useNotificationStore();

onMounted(async () => {
  console.log("trying to connect socket")
  if(!localStorage['token']) return;
  if (!store.connection) {
    const connection = createConnection();
    if (connection) {

        await connection.start();
        connection.on('ReceiveNotifications', function (notifications) {
          console.log(notifications);
        });
        let id: number = Number(localStorage.getItem('UserId'));
        await connection.invoke("SendNotificationToUser", Number(id));
        store.setConnection(connection);
      }
    }
    else {
      console.error('Connection is not created');
    }
});
</script>