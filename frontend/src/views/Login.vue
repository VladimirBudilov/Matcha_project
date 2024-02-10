<script lang="ts" setup>
  import { reactive } from 'vue';

  interface FormState {
	username: string;
	password: string;
	remember: boolean;
  }

  const formState = reactive<FormState>({
	username: '',
	password: '',
	remember: true,
  });
  const onFinish = (values: any) => {
	console.log('Success:', values);
  };

  const onFinishFailed = (errorInfo: any) => {
	console.log('Failed:', errorInfo);
  };

</script>


<template>
	<div class="Login">
		<a-form
		:model="formState"
		name="basic"
		:label-col="{ span: 8 }"
		:wrapper-col="{ span: 16 }"
		autocomplete="off"
		@finish="onFinish"
		@finishFailed="onFinishFailed"
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
		</a-form-item>
		</a-form>
	</div>
	<div class="SignUpButton">
		<a-form-item :wrapper-col="{ offset: 8, span: 16 }">
			<a-button type="primary" html-type="submit">Sign up</a-button>
		</a-form-item>
	</div>
</template>

<style>
.Login {
	position: relative;
	top: 30vh;
}

:where(.css-dev-only-do-not-override-1hsjdkk).ant-form-item .ant-form-item-control-input {
	width: 25vw;
}

:where(.css-dev-only-do-not-override-1hsjdkk).ant-form-item .ant-form-item-label >label {
	color: var(--color-text);
}

.SignUpButton {
	position: relative;
	top: 28vh;
}

</style>
