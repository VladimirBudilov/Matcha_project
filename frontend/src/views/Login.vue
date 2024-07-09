<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import NamePasswordInput from '../components/NamePasswordInput.vue'
import SignUpForm from '@/components/SignUpForm.vue';
import ForgotPasswordForm from '@/components/ForgotPasswordForm.vue';
import { SignUpStore } from '@/stores/SignUpStore'

const IsActiveSignUp = storeToRefs(SignUpStore()).IsActiveSignUp
const IsForgotPassword = storeToRefs(SignUpStore()).IsForgotPassword

</script>


<template>
	<div v-if="!IsActiveSignUp && !IsForgotPassword">
		<NamePasswordInput />
	</div>
	<div v-if="IsActiveSignUp">
		<SignUpForm />
	</div>
	<div v-if="IsForgotPassword">
		<ForgotPasswordForm />
	</div>
</template>

<style>

</style>
