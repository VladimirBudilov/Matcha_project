<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { storeToRefs } from 'pinia';
import { SignUpStore } from '@/stores/SignUpStore';
import createConnection from '@/services/NotificationService'
import axios from 'axios';
import {HubConnection} from "@microsoft/signalr";
import {Notification, NotificationType} from '@/stores/NotoficationStore'

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp
const IsLogin = storeToRefs(SignUpStore()).IsLogin

const notificationConnection = ref<HubConnection>();

defineExpose({
  notificationConnection
});
const SignUpButtonTurnOn = () => {
	IsActiveSignUp.value = !IsActiveSignUp.value
}

interface FormState {
	username: string;
	password: string;
	remember: boolean;
}

interface loginRes {
	error?: string
	token?: string
}


const errorMsg = ref('')

const formState = reactive<FormState>({
	username: '',
	password: '',
	remember: true,
});

const onFinish = async (values: any) => {
	errorMsg.value = ''
	await axios.post('api/auth/login', values).catch((msg) => {
		if (msg.response.data.error) {
			errorMsg.value = msg.response.data.error
		}
	}).then(async (res) => {
		const loginRes : loginRes = res?.data
		if (errorMsg.value == '' && loginRes.token) {
			IsLogin.value = true
			localStorage.setItem("token", loginRes.token)

      axios.defaults.headers.common.Authorization = 'Bearer ' + loginRes.token;

      notificationConnection.value = createConnection();
      if (notificationConnection.value) {
        notificationConnection.value.start()
            .then(() => {
              console.log('Connection started');
              notificationConnection.value?.on('ReceiveNotification', (message: Notification[]) => {
                console.log('ReceiveNotification: ' + message);
              })
            })
            .catch((err: Error) => console.error('Error while starting connection: ' + err));
      }

			window.location.assign('https://' + window.location.host)
		}
	})
};

const onFinishFailed = (errorInfo: any) => {
	console.log('Failed:', errorInfo);
};

</script>


<template>
	<a-form
		:model="formState"
		name="basic"
		:label-col="{ span: 9 }"
		:wrapper-col="{ span: 7 }"
		autocomplete="on"
		@finish="onFinish"
		>
		<a-form-item
			label="Username"
			name="username"
			:rules="[{ required: true, message: 'Please input your username!' }]"
		>
			<a-input v-model:value="formState.username" />
		</a-form-item>

		<a-form-item
			label="Password"
			name="password"
			:rules="[{ required: true, message: 'Please input your password!' }]"
		>
			<a-input-password v-model:value="formState.password" />
		</a-form-item>

		<a-form-item :wrapper-col="{ offset: 9, span: 7 }">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<p style='color: red;'>
				{{ errorMsg }}
			</p>
			<a-button type="primary" html-type="signup" @click="SignUpButtonTurnOn">Sign up</a-button>
		</a-form-item>
	</a-form>

</template>

<style>

</style>
