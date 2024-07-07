
<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { SignUpStore } from '@/stores/SignUpStore'
import { storeToRefs } from 'pinia';
import axios from 'axios';
import { message } from 'ant-design-vue';

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp

const SignUpButtonTurnOn = () => {
	IsActiveSignUp.value = !IsActiveSignUp.value
}

const validateMessages = {
required: '${label} is required!',
types: {
	email: '${label} is not a valid email!',
	number: '${label} is not a valid number!',
}
};

const errorMsg = ref('')

const formState = reactive({
	email: '',
	userName: '',
	firstName: '',
	lastName: '',
	password: ''
});
const onFinish = (values: any) => {
	errorMsg.value = ''
	axios.post('api/auth/registration', values).catch((msg) => {
		if (msg.response.data.error) {
			errorMsg.value = msg.response.data.error
		}
		else {
			errorMsg.value = msg.response.data
		}
		message.error(errorMsg.value);
	}).then(() => {
		if (errorMsg.value == '') {
			IsActiveSignUp.value = !IsActiveSignUp.value
		}
	})
};
</script>


<template>
	<a-card id="SignUpForm">

	<a-form
		:model="formState"
		:label-col="{ span: 9 }"
		:wrapper-col="{ span: 7 }"
		name="nest-messages"
		:validate-messages="validateMessages"
		@finish="onFinish"
	>


		<a-form-item :name="['email']" label="Email" :rules="[{ type: 'email', required: true }]">
			<a-input v-model:value="formState.email" />
		</a-form-item>
		<a-form-item :name="['userName']" label="Username" :rules="[{type: 'string', required: true }]">
		<a-input v-model:value="formState.userName" />
		</a-form-item>
		<a-form-item :name="['firstName']" label="First name" :rules="[{ type: 'string', required: true }]">
		<a-input v-model:value="formState.firstName" />
		</a-form-item>
		<a-form-item :name="['lastName']" label="Last name" :rules="[{ type: 'string', required: true }]">
		<a-input v-model:value="formState.lastName" />
		</a-form-item>
		<a-form-item :name="['password']" label="Password" :rules="[{ type: 'string', required: true }]">
		<a-input-password v-model:value="formState.password" />
		</a-form-item>
		<a-form-item id="button-signup-submit-cancel">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<a-button id="button-singup-cancel" danger type="primary"  html-type="cancel" @click="SignUpButtonTurnOn">Cancel</a-button>
		</a-form-item>


	</a-form>
	</a-card>

</template>

<style>
#SignUpForm {
	position: fixed;
	width: 50vw;

	margin-top: 10vh;
	margin-left: 20vw;
	margin-right: 20vw;
	margin-bottom: 10vh;
	background-color: var(--color-background-soft);
	padding-top: 10vh;
	padding-bottom: 4vh;
	overflow: auto;
}

#button-signup-submit-cancel {
	position: relative;
	margin-left: 18vw;
}

#button-singup-cancel {
	margin-top: 3px
}

@media screen and (max-width: 1100px) {
	#SignUpForm {
		height: 88vh;
	}

}
</style>
