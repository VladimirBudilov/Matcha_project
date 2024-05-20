<script setup lang="ts">
import axios from 'axios';
import { onMounted } from 'vue';


onMounted(async () => {
	await axios.get('api/auth/get-id').then((res) => {
		localStorage.setItem('UserId', String(res?.data))
	})

	axios.get('api/profile').then((res) => {
		console.log(res?.data)
	})
})

</script>

<template>

</template>

<style>

</style>
