<script setup lang="ts">
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';

const profiles = storeToRefs(SignUpStore()).profiles


const getViewList = async () => {
	await axios.get('api/actions/viewed').catch(() => {
		message.error(`Fill out the profile!`);
	}).then(res => {
		if (res.data){
			profiles.value = res.data.data
		}
	})
}

const columns = [
	{
	title: 'Avatar',
	key: 'avatar',
	},
	{
	title: 'Name',
	key: 'name',
	}
]

onMounted(async () => {
	profiles.value.length = 0
	await getViewList()
})

</script>

<template>
	<div id="views" v-if="profiles.length">
		<a-table :dataSource="profiles" :columns="columns" :pagination="false">
		<template #bodyCell="{ column, record }">
			<template v-if="column.key === 'avatar'">
				<a-image v-if="record.profilePicture.picture"
				:width="50"
				:src="'data:image/*' + ';base64,' + record.profilePicture.picture"
				/>
			</template>
			<template v-if="column.key === 'name'">
				<RouterLink :to="'/users/' + record.profileId"> {{ record.firstName + ' ' + record.lastName }} </RouterLink>
			</template>
		</template>
	</a-table>
	</div>


</template>

<style>
#views {
	position: fixed;
	width: 50vw;
	height: 85vh;
	padding-top: 7vh;
	padding-left: 1vw;
	padding-right: 1vw;
	overflow: auto;
}

@media screen and (max-width: 1100px) {
	#views {
		width: 100vw;
	}
}

</style>
