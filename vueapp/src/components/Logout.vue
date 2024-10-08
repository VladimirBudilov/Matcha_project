<script setup lang="ts">
import axios from 'axios';
import {storeToRefs} from 'pinia';
import {SignUpStore} from '@/stores/SignUpStore';
import {useNotificationStore} from '@/stores/NotoficationStore'
import {HubConnectionState} from "@microsoft/signalr";

const IsLogin = storeToRefs(SignUpStore()).IsLogin

const connection = useNotificationStore().connection;

const LogoutButtonTurnOn = () => {
	axios.get('/api/auth/logout').then(() => {
    if(connection?.state === HubConnectionState.Connected)
    {
      connection.stop()
    }
		localStorage.removeItem('token')
		localStorage.removeItem('UserId')
		IsLogin.value = false
		window.location.assign(window.location.origin + '/login')
	})
}
</script>

<template>
	<a-button id="logout" type="text" size="large" html-type="signup" @click="LogoutButtonTurnOn">Logout</a-button>
</template>

<style>
#logout {
	font-size: 25px;
	color:firebrick
}

</style>
