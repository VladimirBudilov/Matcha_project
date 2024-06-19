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

	console.log(getProfileParams.value)
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

			console.log(profiles.value)
		}
	})
}

const LeaveChat = async () => {
	await connection.value?.invoke("LeaveChat", chatId.value[1])
        .then(() => {
            console.log('Chat left')
        })
        .catch(err => console.error(err.toString()));
}

const setChatId = async (userId : number) => {
	if (chatId.value[1]) {
		await LeaveChat()
	}
	chatId.value[0] = Number(localStorage.getItem('UserId'))
	chatId.value[1] = userId
	console.log("set ids for chat" + chatId.value)
}

onMounted(async () => {
	await GetFilters();
	await GetProfile();
	connection.value = createConnection();
  connection.value.start().catch(err => message.error(err.toString()));
  connection.value.on("ReceiveMessage", (user, message) => {
        console.log('Message received by ' + user + ' ' + message);
    });
})

watch(
	() => getProfileParams.value.pagination.pageNumber,
	async () => {
		console.log('current', getProfileParams.value.pagination.pageNumber);
		await GetProfile()
	}
);



</script>

<template>
	<div id="chat">
		<a-table :dataSource="profiles" :columns="columns" :pagination="false">
			<template #bodyCell="{ column, record }">
				<template v-if="column.key === 'avatar'">
					<a-image
					:width="50"
					:src="'data:image/*' + ';base64,' + record.profilePicture"
					/>
				</template>
				<template v-if="column.key === 'name'">
					<a @click="setChatId(record.profileId)">
						{{ record.firstName + ' ' + record.lastName }}
					</a>
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
	padding-top: 7vh;
	padding-bottom: 10vh;
	padding-left: 1vw;
	padding-right: 70vw;
	width: 98vw;
}

#pagination-users-chat {
	position: relative;
	margin-top: 10vh;
	width: 100%;
	bottom: 3vh;
	padding-top: 2vh;
	padding-bottom: 2vh;
	padding-left: 3vh;
	padding-right: 3vh;
	background-color: grey;
	color:black
}

</style>
