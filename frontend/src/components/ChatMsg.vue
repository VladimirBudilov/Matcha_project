<script setup lang="ts">
import dayjs from 'dayjs';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { ref, watch } from 'vue';
import relativeTime from 'dayjs/plugin/relativeTime';
dayjs.extend(relativeTime);


const messages = storeToRefs(SignUpStore()).messages
const connection = storeToRefs(SignUpStore()).connection
const chatId = storeToRefs(SignUpStore()).chatId

const msg = ref<string>('')

const GetMessages = async () => {
	await axios.post('api/actions/chat',  {producerId: chatId.value[0], consumerId: chatId.value[1]})
	.then((res) => {
	console.log(res.data);
	messages.value = res.data;
	})
	.catch(err => message.error(err.toString()));
}

const StartChat = async () => {
	await connection.value?.invoke("StartChat", Number(chatId.value[1]))
		.then((data) => {
			message.open(data);
		})
		.catch(err => message.error(err.toString()));
}

const SendMsg = async () => {
	if (msg.value != ''){
		connection.value?.invoke("SendMessage", chatId.value[1], msg.value)
		.catch(err => message.error(err.toString()));
        msg.value = '';
	}
}

watch (
	() => chatId.value[1],
	async () => {
		console.log('current', chatId.value);
		await StartChat()
		await GetMessages()
	}
)

</script>

<template>
	<div id="chat-msg" v-if="messages.length">
		<a-list
		class="comment-list"
		item-layout="horizontal"
		:data-source="messages"
		>
		<template #renderItem="{ item }">
			<a-comment>
				<template #author>{{ item.id }}</template>
				<template #content>
				<p>
					{{ item.text }}
				</p>
				</template>
				<template #datetime>
				<a-tooltip :title="item.date.format('YYYY-MM-DD HH:mm:ss')">
					<span>{{ item.date.fromNow() }}</span>
				</a-tooltip>
				</template>
			</a-comment>
		</template>
		</a-list>
	</div>
	<div id="chat-msg-input" v-if="chatId[1]">
		<a-textarea v-model:value="msg" placeholder="Write msg" :rows="2" />
		<a-button type="primary" html-type="signup" @click="SendMsg">Send</a-button>

	</div>

</template>

<style>

</style>
