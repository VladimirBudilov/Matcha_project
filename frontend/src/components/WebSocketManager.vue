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
      await connection.start()
        .then(() => {
          console.log('Connection started');
          connection.on('ReceiveNotification', (message: Notification[]) => {
            console.log('ReceiveNotification: ' + message);
          });
        })
        .catch((err: Error) => console.error('Error while starting connection: ' + err));
      store.setConnection(connection);
      console.log("")
    }
    else
    {
      console.error('Connection is not created');
    }
  }
  else
  {
    console.log("Connection already exists")
    console.log(store.connection)
    await store.connection.start();
  }
});
</script>