<script setup lang="ts">
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { SignUpStore } from '@/stores/SignUpStore';
import {useNotificationStore} from '@/stores/NotoficationStore'
const IsLogin = storeToRefs(SignUpStore()).IsLogin

var notificationConnection = useNotificationStore().notificationConnection;

const LogoutButtonTurnOn = () => {
	axios.get('api/auth/logout').then(() => {
    if(notificationConnection)
    {
      notificationConnection.stop()
    }
		localStorage.removeItem('token')
		localStorage.removeItem('UserId')
		IsLogin.value = false
		window.location.assign('https://' + window.location.host + '/login')
	})
}
</script>

<template>
	<a-button type="text" size="large" html-type="signup" @click="LogoutButtonTurnOn" style="font-size: 25px; color:firebrick">Logout</a-button>
</template>

<style>

</style>
