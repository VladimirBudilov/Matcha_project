<script setup lang="ts">
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';

const profiles = storeToRefs(SignUpStore()).profiles

const getBlockList = async () => {
	await axios.get('api/actions/blacklist').then(res => {
		if (res.data){
			profiles.value = res.data.data
		}
	})
}

const block = async (profileId: number) => {
	await axios.post('api/actions/block', {producerId: Number(localStorage.getItem('UserId')), consumerId: profileId}).catch((res) => {
		if (res.response.data.error) {
			message.error(`Error: ${res.response.data.error} ${localStorage.getItem('UserId')}`);
		}
		else {
			message.error(`Fill out the profile!`);
		}
	}).then((res) => {
		if (res?.data) {
			if (res?.data) {
				message.success(`You unblocked!`);
				profiles.value.forEach(function(item, index, object) {
					if (item.profileId == profileId) {
						object.splice(index, 1);
					}
				})
			}
		}
	})
}

onMounted(async () => {
	profiles.value.length = 0
	await getBlockList()
})

const columns = [
	{
	title: 'Avatar',
	key: 'avatar',
	},
	{
	title: 'Name',
	key: 'name',
	},
	{
	key: 'unblock',
	},
]

</script>

<template>
	<div id="block" v-if="profiles.length">
		<a-table :dataSource="profiles" :columns="columns" :pagination="false">
		<template #bodyCell="{ column, record }">
			<template v-if="column.key === 'avatar'">
				<a-image v-if="record.profilePicture.picture"
				:width="50"
				:src="'data:image/*' + ';base64,' + record.profilePicture.picture"
				/>
			</template>
			<template v-if="column.key === 'name'">
				<RouterLink :to="'/user/' + record.profileId">{{ record.firstName + ' ' + record.lastName }}</RouterLink>
			</template>
			<template v-if="column.key === 'unblock'">
				<a-button @click="block(record.profileId)">
					unblock
				</a-button>
			</template>
		</template>
	</a-table>
	</div>


</template>

<style>
#block{
	position: fixed;
	width: 50vw;
	height: 85vh;
	padding-top: 7vh;
	padding-left: 1vw;
	padding-right: 1vw;
	overflow: auto;
}

@media screen and (max-width: 1100px) {
	#block {
		width: 100vw;
	}
}

</style>
