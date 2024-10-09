<script setup lang="ts">
import dayjs from 'dayjs';
import { SignUpStore } from '@/stores/SignUpStore';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { storeToRefs } from 'pinia';
import { onMounted, ref, watch } from 'vue';
import relativeTime from 'dayjs/plugin/relativeTime';
dayjs.extend(relativeTime);


const messages = storeToRefs(SignUpStore()).messages
const connection = storeToRefs(SignUpStore()).connection
const chatId = storeToRefs(SignUpStore()).chatId

const msg = ref<string>('')

const GetMessages = async () => {
	await axios.post('/api/actions/chat',  {producerId: chatId.value[0], consumerId: chatId.value[1]})
	.then((res) => {
		messages.value = res.data.data;
		messages.value.forEach(el => el.datetime = dayjs(el.date).format('YYYY-MM-DD HH:mm:ss'))
	})
	.catch(err => message.error(err.toString()));
	const document_chat = document.getElementById('chat-msg')
	if (document_chat) {
		document_chat.scrollTop = document_chat.scrollHeight + 20

	}

}

const ReceiveMessage = async () => {
	if (connection.value) {
		connection.value.on("ReceiveMessage", (user, message) => {
      		messages.value.push({ author: user, content: message, datetime: dayjs().format('YYYY-MM-DD HH:mm:ss') });
				const document_chat = document.getElementById('chat-msg')
				if (document_chat) {
					setTimeout(() => {
						document_chat.scrollTop = document_chat.scrollHeight + 20
					}, 100)
				}
			});
	}


}

const StartChat = async () => {
	await connection.value?.invoke("StartChat", Number(chatId.value[1]))
		.then((data) => {
			if(data != null)
      {
        message.open(data);
      }
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

onMounted(async () => {
	messages.value = []
})

watch (
	() => chatId.value[1],
	async () => {
		if (chatId.value[1]) {
			await GetMessages()
			await StartChat()
			await ReceiveMessage()
		}

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
						<a-list-item class="ant-list-item-custom">
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
	</div>
	<div id="chat-msg-input" v-if="chatId[1]">
		<a-textarea v-model:value="msg" placeholder="Write msg" :rows="2" />
		<a-button type="primary" html-type="signup" @click="SendMsg">Send</a-button>
	</div>
</template>

<style>
#chat-msg {
  position: fixed;
  top: 9vh;
  bottom: 20vh;
  left: 50vw;
  right: 1vw;
  width: 50vw;
  height: 70vh;
  overflow-y: auto;
  scroll-margin-top: 0;
  scroll-padding-top: 0;
  color: black; /* Ensure text color is black */
}

#chat-msg .ant-list-item-custom {
  color: black; /* Ensure text color is black */
}

#chat-msg-input {
  position: fixed;
  bottom: 5vh;
  width: 50vw;
  left: 50vw;
}

#chat-msg p {
  color: black !important; /* Ensure text color is black */
}

</style>
