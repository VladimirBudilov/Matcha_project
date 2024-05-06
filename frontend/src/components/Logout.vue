<script setup lang="ts">
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { SignUpStore } from '@/stores/SignUpStore';

const IsLogin = storeToRefs(SignUpStore()).IsLogin

const LogoutButtonTurnOn = () => {
	axios.get('api/auth/logout').then(() => {
		localStorage.removeItem('token')
		IsLogin.value = false
		window.location.assign('https://' + window.location.host + '/login')
	})
}
</script>

<template>
	<div class="Logout">
		<a-button type="primary" html-type="signup" @click="LogoutButtonTurnOn">Logout</a-button>
	</div>
</template>

<style>

</style>
