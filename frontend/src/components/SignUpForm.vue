
<script lang="ts" setup>
import { reactive } from 'vue';
import { SignUpStore } from '@/stores/SignUpStore'
import { storeToRefs } from 'pinia';
import axios from 'axios';

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp

const SignUpButtonTurnOn = () => {
	IsActiveSignUp.value = !IsActiveSignUp.value
}

const layout = {
labelCol: { span: 8 },
wrapperCol: { span: 16 },
};

const validateMessages = {
required: '${label} is required!',
types: {
	email: '${label} is not a valid email!',
	number: '${label} is not a valid number!',
}
};

const formState = reactive({
user: {
	email: '',
	username: '',
	firstName: '',
	lastName: '',
	password: ''
},
});
const onFinish = (values: any) => {
	axios.post('api/user')
	console.log('Success:', values);
};
</script>


<template>
	<a-form
		:model="formState"
		v-bind="layout"
		name="nest-messages"
		:validate-messages="validateMessages"
		@finish="onFinish"
	>
		<a-form-item :name="['user', 'email']" label="Email" :rules="[{ type: 'email', required: true }]">
			<a-input v-model:value="formState.user.email" />
		</a-form-item>
		<a-form-item :name="['user', 'username']" label="Username" :rules="[{type: 'string', required: true }]">
		<a-input v-model:value="formState.user.username" />
		</a-form-item>
		<a-form-item :name="['user', 'firstName']" label="First name" :rules="[{ type: 'string', required: true }]">
		<a-input v-model:value="formState.user.firstName" />
		</a-form-item>
		<a-form-item :name="['user', 'lastName']" label="Last name" :rules="[{ type: 'string', required: true }]">
		<a-input v-model:value="formState.user.lastName" />
		</a-form-item>
		<a-form-item :name="['user', 'password']" label="Password" :rules="[{ type: 'string', required: true }]">
		<a-input-password v-model:value="formState.user.password" />
		</a-form-item>
		<a-form-item :wrapper-col="{ ...layout.wrapperCol, offset: 8 }">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<a-button danger type="primary"  html-type="cancel" @click="SignUpButtonTurnOn" style="margin-left: 1vw;">Cancel</a-button>
		</a-form-item>
	</a-form>
</template>
