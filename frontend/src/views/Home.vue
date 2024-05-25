<script setup lang="ts">
import axios from 'axios';
import { onMounted } from 'vue';
import { message } from 'ant-design-vue';


onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})

	axios.get('api/profile').catch((res) => {
		if (res.code == 403) {
			message.success(`Fill out the profile!`);
		}
	}).then((res) => {
		if (res?.data){
			console.log(res?.data)
		}
	})
})

</script>

<template>

</template>

<style>

</style>
