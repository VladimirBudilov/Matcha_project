<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { storeToRefs} from 'pinia';
import { SignUpStore } from '@/stores/SignUpStore';
import axios from 'axios';
import { message } from 'ant-design-vue';

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp
const IsForgotPassword = storeToRefs(SignUpStore()).IsForgotPassword

const SignUpButtonTurnOn = () => {
  IsActiveSignUp.value = !IsActiveSignUp.value
}

const ForgotPasswordButtonTurnOn = () => {
	IsForgotPassword.value = !IsForgotPassword.value
}

const IsLogin = storeToRefs(SignUpStore()).IsLogin


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
		else {
			errorMsg.value = msg.response.data
		}
		message.error(errorMsg.value);
	}).then(async (res) => {
		const loginRes : loginRes = res?.data
		if (errorMsg.value == '' && loginRes.token) {
			IsLogin.value = true
			localStorage.setItem("token", loginRes.token)

      axios.defaults.headers.common.Authorization = 'Bearer ' + loginRes.token;
      window.location.assign('https://' + window.location.host)
		}
	})
};

</script>


<template>
	<a-card id="Login">
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

		<a-form-item id="button-submit-signup">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<a-button id='button-signup' type="primary" html-type="signup" @click="SignUpButtonTurnOn">Sign up</a-button>
			<a-button id='button-forgot-password' type="primary" html-type="submit" @click="ForgotPasswordButtonTurnOn">Forgot password?</a-button>
		</a-form-item>
	</a-form>
	</a-card>
</template>

<style>
#Login {
	position: fixed;
	width: 50vw;
	margin-top: 10vh;
	margin-left: 20vw;
	margin-right: 20vw;
	margin-bottom: 10vh;
	background-color: var(--color-background-soft);
	padding-top: 10vh;
	padding-bottom: 4vh;
}

#button-submit-signup {
	position: relative;
	margin-left: 18vw;
}

#button-signup {
	margin-top: 3px
}

#button-forgot-password{
	margin-top: 3px
}

@media screen and (max-width: 1100px) {
	#button-submit-signup {
		position: relative;
		margin-left: inherit;
	}
}

</style>
