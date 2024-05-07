<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { storeToRefs } from 'pinia';
import { SignUpStore } from '@/stores/SignUpStore';
import axios from 'axios';

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp
const IsLogin = storeToRefs(SignUpStore()).IsLogin


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
const onFinish = (values: any) => {
	errorMsg.value = ''
	axios.post('api/auth/login', values).catch((msg) => {
		if (msg.response.data.error) {
			errorMsg.value = msg.response.data.error
		}
	}).then((res) => {
		const loginRes : loginRes = res?.data
		if (errorMsg.value == '' && loginRes.token) {
			IsLogin.value = true
			localStorage.setItem("token", loginRes.token)
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
		:label-col="{ span: 8 }"
		:wrapper-col="{ span: 16 }"
		autocomplete="on"
		@finish="onFinish"
		>
		<a-form-item style="color: var(--color-text);"
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

		<a-form-item :wrapper-col="{ offset: 8, span: 16 }">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<p style='color: red;'>
				{{ errorMsg }}
			</p>
			<a-button type="primary" html-type="signup" @click="SignUpButtonTurnOn">Sign up</a-button>
		</a-form-item>
	</a-form>



</template>

<style>
:where(.css-dev-only-do-not-override-1hsjdkk).ant-form {
    margin-left: 10%;
	margin-right: 10%;
    padding-left: 10%;
	padding-right: 10%;
}

:where(.css-dev-only-do-not-override-1hsjdkk).ant-form-item .ant-form-item-label >label {
	color: var(--color-text);
}

</style>
