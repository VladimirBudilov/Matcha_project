<script setup lang="ts">
import axios from 'axios';
import { onMounted } from 'vue';


onMounted(async () => {
	axios.get('api/profile').then((res) => {
		console.log(res?.data)
	})
})

</script>

<template>

</template>

<style>

</style>
