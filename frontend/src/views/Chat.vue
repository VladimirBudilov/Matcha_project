<script setup lang="ts">
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';
import createConnection from "../services/ChatService";
import ChatMsg from "@/components/ChatMsg.vue"

const getFilters = storeToRefs(SignUpStore()).getFilters
const getProfileParams = storeToRefs(SignUpStore()).getProfileParams
const profiles = storeToRefs(SignUpStore()).profiles
const connection = storeToRefs(SignUpStore()).connection
const chatId = storeToRefs(SignUpStore()).chatId


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
	title: 'Age',
	dataIndex: 'age',
	key: 'age',
	},
	{
	key: 'block',
	},
]

const GetFilters = async () => {
	await axios.get('api/profiles/filters').catch((res) => {
		if (res.response.data) {
			message.error(res.response.data)
		}
	}).then ((res) => {
		if (res?.data) {
			getFilters.value = res.data
			getFilters.value.maxDistance = Math.floor(getFilters.value.maxDistance)
		}
	})
}

const GetProfile = async () => {
	getProfileParams.value.search.maxDistance = getFilters.value.maxDistance
	getProfileParams.value.search.minAge = getFilters.value.minAge
	getProfileParams.value.search.maxAge = getFilters.value.maxAge
	getProfileParams.value.search.minFameRating = getFilters.value.minFameRating
	getProfileParams.value.search.maxFameRating = getFilters.value.maxFameRating
	getProfileParams.value.search.isMatched = true
	getProfileParams.value.search.sexualPreferences = null
	getProfileParams.value.search.commonTags = []

	await axios.post('api/profiles', getProfileParams.value).catch((res) => {
		if (res.response.data) {
			message.error(res.response.data)
		}
		else if (res.response) {
			message.error(res.response);
		}
	}).then((res) => {
		if (res?.data){
			profiles.value = res.data.profiles
			if (getProfileParams.value.pagination.pageSize) {
				getProfileParams.value.pagination.total = getProfileParams.value.pagination.pageSize * res.data.amountOfPages
			}

		}
	})
}

const LeaveChat = async () => {
	await connection.value?.invoke("LeaveChat", chatId.value[1])
        .then(() => {

        })
        .catch(err => console.error(err.toString()));
}

const setChatId = async (userId : number) => {
	if (chatId.value[1]) {
		await LeaveChat()
	}
	chatId.value[0] = Number(localStorage.getItem('UserId'))
	chatId.value[1] = userId
}

const block = async (profileId: number) => {
	await axios.post('api/actions/block', {producerId: Number(localStorage.getItem('UserId')), consumerId: profileId}).catch((res) => {
		message.error(`Error: ${res.response.data.error} ${localStorage.getItem('UserId')}`);
	}).then((res) => {
		if (res?.data) {
			if (res?.data) {
				message.success(`You blocked!`);
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
	await GetFilters();
	await GetProfile();
	getProfileParams.value.pagination.pageNumber = 1
	chatId.value[1] = 0
	connection.value = createConnection();
	connection.value.start().catch(err => message.error(err.toString()));
})

watch(
	() => getProfileParams.value.pagination.pageNumber,
	async () => {
		await GetProfile()
	}
);



</script>

<template>
	<div id="chat" v-if="profiles.length">
		<a-table :dataSource="profiles" :columns="columns" :pagination="false">
			<template #bodyCell="{ column, record }">
				<template v-if="column.key === 'avatar'">
					<a-image v-if="typeof(record.profilePicture) === 'string'"
					:width="50"
					:src="'data:image/*' + ';base64,' + record.profilePicture"
					/>
				</template>
				<template v-if="column.key === 'name'">
					<a @click="setChatId(record.profileId)">
						{{ record.firstName + ' ' + record.lastName }}
					</a>
				</template>
				<template v-if="column.key === 'block'">
					<a-button @click="block(record.profileId)">
						block
					</a-button>
				</template>
			</template>
		</a-table>
		<a-pagination
		id="pagination-users-chat"
		v-model:current="getProfileParams.pagination.pageNumber"
		:total="getProfileParams.pagination.total" show-less-items
		:defaultPageSize="getProfileParams.pagination.pageSize"/>
	</div>
	<ChatMsg />


</template>

<style>
#chat {
	position: fixed;
	width: 50vw;
	height: 85vh;
	padding-top: 7vh;
	padding-left: 1vw;
	padding-right: 1vw;
	overflow: auto;
}

#pagination-users-chat {
	position: fixed;
	bottom: 3vh;
	width: 45vw;
	padding-top: 1vh;
	padding-bottom: 2vh;
	padding-left: 1vh;
	background-color: var(--color-background-soft);
	color:var(--color-text);
	display: flex;
	flex-wrap: wrap;
}

</style>
