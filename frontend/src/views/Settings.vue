<script lang="ts" setup>
import { reactive, ref } from 'vue';
import axios from 'axios';

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
		errorMsg.value = 'Your new password are not the same'
	}
	else {
		await axios.put('api/user/' + localStorage.getItem('UserId') + '/update-password', values).catch((msg) => {
			if (msg.response.data.error) {
				errorMsg.value = msg.response.data.error
			}
		}).then(async (res) => {
			if (errorMsg.value === '')
				errorMsg.value = 'Success!'
		})
	}

};

</script>


<template>
	<a-form class="Settings"
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

			<p v-if='errorMsg == "Success!"' style='color: green;'>
				{{ errorMsg }}
			</p>
			<p v-else style='color: red;'>
				{{ errorMsg }}
			</p>
		</a-form-item>
	</a-form>



</template>

<style>
.Settings{
	position: relative;
	padding-top: 7vh;
	padding-bottom: 4vh;
}
</style>
