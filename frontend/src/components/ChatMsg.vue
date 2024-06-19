<script setup lang="ts">
import dayjs from 'dayjs';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch } from 'vue';
import relativeTime from 'dayjs/plugin/relativeTime';
dayjs.extend(relativeTime);

const author = ref('')

async function GetProfileInfo(){
	await axios.get('api/profiles/' + localStorage.getItem('UserId')).then((res) => {
		author.value = res.data.firstName + ' ' + res.data.lastName
	})
}


const messages = storeToRefs(SignUpStore()).messages
const connection = storeToRefs(SignUpStore()).connection
const chatId = storeToRefs(SignUpStore()).chatId

const msg = ref<string>('')

const GetMessages = async () => {
	messages.value

	await axios.post('api/actions/chat',  {producerId: chatId.value[0], consumerId: chatId.value[1]})
	.then((res) => {
		console.log(res.data);
		messages.value = res.data.data;
		messages.value.forEach(el => el.datetime = dayjs(el.date).format('YYYY-MM-DD HH:mm:ss'))
	})
	.catch(err => message.error(err.toString()));
}

const ReceiveMessage = async () => {
	if (connection.value) {
		connection.value.on("ReceiveMessage", (user, message) => {
      		messages.value.push({ author: author.value, content: message, datetime: dayjs().format('YYYY-MM-DD HH:mm:ss') });
    	});
	}


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

onMounted (async () => {
	await GetProfileInfo()
})

watch (
	() => chatId.value[1],
	async () => {
		console.log('current', chatId.value);
		await StartChat()
		await GetMessages()
		await ReceiveMessage()
	}
)

</script>

<template>
	<div id="chat-msg">
		<div id="chat-msgs" v-if="messages.length">
			<a-card>
				<a-list
				class="comment-list"
				item-layout="horizontal"
				:data-source="messages"
				>
					<template #renderItem="{ item }">
						<a-list-item>
							<a-comment>
								<template #author>{{ item.author }}</template>
								<template #content>
								<p>
									{{ item.content }}
								</p>
								</template>
								<template #datetime>
									<span>{{ item.datetime }}</span>
								</template>
							</a-comment>
						</a-list-item>
					</template>
				</a-list>
			</a-card>
		</div>
		<div id="chat-msg-input" v-if="chatId[1]">
			<a-textarea v-model:value="msg" placeholder="Write msg" :rows="2" />
			<a-button type="primary" html-type="signup" @click="SendMsg">Send</a-button>
		</div>
	</div>
</template>

<style>
#chat-msg {
	position:absolute;
	top: 9vh;
	bottom: 20vh;
	left: 30vw;
	right: 1vw;
	width: 50vw;
	height: 80vh;
	overflow-y: auto;
	scroll-margin-top: 0;
	scroll-padding-top: 0;

}

</style>
