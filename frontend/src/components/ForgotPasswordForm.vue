
<script lang="ts" setup>
import { reactive, ref } from 'vue';
import { SignUpStore } from '@/stores/SignUpStore'
import { storeToRefs } from 'pinia';
import axios from 'axios';
import { message } from 'ant-design-vue';

const IsForgotPassword = storeToRefs(SignUpStore()).IsForgotPassword

const ForgotPasswordButtonTurnOn = () => {
	IsForgotPassword.value = !IsForgotPassword.value
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
});
const onFinish = (values: any) => {
	errorMsg.value = ''
	axios.post('api/auth/restore-password', values).catch((msg) => {
		if (msg.response.data.error) {
			errorMsg.value = msg.response.data.error
		}
		else {
			errorMsg.value = msg.response.data
		}
		message.error(errorMsg.value);
	}).then(() => {
		if (errorMsg.value == '') {
			ForgotPasswordButtonTurnOn()
		}
	})
};
</script>


<template>
	<a-card id="ForgotPasswordForm">

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
		<a-form-item id="button-forgot-password-submit-cancel">
			<a-button type="primary" html-type="submit">Submit</a-button>
			<a-button id="button-forgot-password-cancel" danger type="primary"  html-type="cancel" @click="ForgotPasswordButtonTurnOn">Cancel</a-button>
		</a-form-item>


	</a-form>
	</a-card>

</template>

<style>
#ForgotPasswordForm {
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

#button-forgot-password-submit-cancel {
	position: relative;
	margin-left: 18vw;
}

#button-forgot-password-cancel {
	margin-top: 3px
}

@media screen and (max-width: 1100px) {
	#ForgotPasswordForm {
		height: inherit;
	}

}
</style>
