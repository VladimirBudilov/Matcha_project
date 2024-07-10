<script lang="ts" setup>
import { reactive, ref } from 'vue';
import axios from 'axios';
import { message } from 'ant-design-vue';

interface FormState {
	oldPassword: string;
	newPassword: string;
	rememberPassword: string;
}

const errorMsg = ref('')

const formState = reactive<FormState>({
	oldPassword: '',
	newPassword: '',
	rememberPassword: '',
});

const onFinish = async (values: FormState) => {
	errorMsg.value = ''
	if (values.newPassword !== values.rememberPassword) {
		message.error('Your new password are not the same')
	}
	else {
		await axios.put('api/user/' + localStorage.getItem('UserId') + '/update-password', values).catch((msg) => {
			errorMsg.value = 'Error'
			if (msg.response.data.error) {
				message.error(msg.response.data.error)
			}
			else {
				message.error('Error')
			}
		}).then(async (res) => {
			if (errorMsg.value === '')
				message.success('Success')
		})
	}

};

</script>


<template>
	<a-card id="Settings">
		<a-form
		:model="formState"
		name="basic"
		:label-col="{ span: 9 }"
		:wrapper-col="{ span: 7 }"
		autocomplete="on"
		@finish="onFinish"
		>
		<a-form-item style="color: var(--color-text);"
			label="Old password"
			name="oldPassword"
			:rules="[{ required: true, message: 'Please input your old password!' }]"
		>
			<a-input-password v-model:value="formState.oldPassword" />
		</a-form-item>

		<a-form-item
			label="New password"
			name="newPassword"
			:rules="[{ required: true, message: 'Please input your new password!' }]"
		>
			<a-input-password v-model:value="formState.newPassword" />
		</a-form-item>

		<a-form-item
			label="New password again"
			name="rememberPassword"
			:rules="[{ required: true, message: 'Please input your new password again!' }]"
		>
			<a-input-password v-model:value="formState.rememberPassword" />
		</a-form-item>

		<a-form-item :wrapper-col="{ offset: 9, span: 7 }">
			<a-button type="primary" html-type="submit">Submit</a-button>
		</a-form-item>
	</a-form>
	</a-card>




</template>

<style>
#Settings{
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
</style>
